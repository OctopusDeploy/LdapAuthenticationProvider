using Octopus.Data;
using Octopus.Data.Model.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.HostServices;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;
using Octopus.Server.Extensibility.Authentication.Resources.Identities;
using Octopus.Server.Extensibility.Results;
using System;
using System.Linq;
using System.Threading;
using Novell.Directory.Ldap;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapCredentialValidator : ILdapCredentialValidator
    {
        readonly ISystemLog log;
        readonly ILdapObjectNameNormalizer objectNameNormalizer;
        readonly IUpdateableUserStore userStore;
        readonly ILdapConfigurationStore configurationStore;
        readonly IIdentityCreator identityCreator;
        readonly ILdapService ldapService;

        internal static string EnvironmentUserDomainName = Environment.UserDomainName;

        public string IdentityProviderName => LdapAuthentication.ProviderName;
        public int Priority => 100;

        public LdapCredentialValidator(
            ISystemLog log,
            ILdapObjectNameNormalizer objectNameNormalizer,
            IUpdateableUserStore userStore,
            ILdapConfigurationStore configurationStore,
            IIdentityCreator identityCreator,
            ILdapService ldapService)
        {
            this.log = log;
            this.objectNameNormalizer = objectNameNormalizer;
            this.userStore = userStore;
            this.configurationStore = configurationStore;
            this.identityCreator = identityCreator;
            this.ldapService = ldapService;
        }

        public IResultFromExtension<IUser> ValidateCredentials(string username, string password, CancellationToken cancellationToken)
        {
            if (!configurationStore.GetIsEnabled())
                return ResultFromExtension<IUser>.ExtensionDisabled();

            if (string.IsNullOrWhiteSpace(username))
                return ResultFromExtension<IUser>.Failed("No username provided");

            try
            {
                log.Verbose($"Validating credentials provided for '{username}'...");

                var result = ldapService.ValidateCredentials(username, password, cancellationToken);

                return string.IsNullOrWhiteSpace(result.ValidationMessage)
                    ? GetOrCreateUser(result, cancellationToken)
                    : ResultFromExtension<IUser>.Failed(result.ValidationMessage);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed validating credentials with LDAP provider.");
                return ResultFromExtension<IUser>.Failed("Unable to validate credentials with LDAP provider.");
            }
        }

        public IResultFromExtension<IUser> GetOrCreateUser(string username, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(username))
                return ResultFromExtension<IUser>.Failed("No username provided");

            try
            {
                var result = ldapService.FindByIdentity(username);

                return string.IsNullOrWhiteSpace(result.ValidationMessage)
                    ? GetOrCreateUser(result, cancellationToken)
                    : ResultFromExtension<IUser>.Failed(result.ValidationMessage);
            }
            catch (LdapException ex)
            {
                log.Error(ex, "Failed while getting or creating the user.");
                return ResultFromExtension<IUser>.Failed(username);
            }
        }

        IResultFromExtension<IUser> GetOrCreateUser(UserValidationResult principal, CancellationToken cancellationToken)
        {
            var externalIdentity = principal.ExternalIdentity;
            var displayName = principal.DisplayName;
            var emailAddress = principal.EmailAddress;
            var userPrincipalName = principal.UserPrincipalName;

            const string attributeErrorTemplate = "Octopus is configured to use the '{0}' attribute as the {1} for LDAP users. " +
                                                  "Please make sure this user has a valid '{0}' attribute.";
            const string failMessageTemplate = "We were unable to find a valid {0} when attempting to sign in with LDAP. Please contact your administrator to resolve this.";

            if (string.IsNullOrWhiteSpace(externalIdentity))
            {
                log.Error($"We couldn't find a valid external identity to use for the LDAP user '{displayName}' with email address '{emailAddress}' for the user account named '{userPrincipalName}'. "
                          + string.Format(attributeErrorTemplate, configurationStore.GetUsernameAttribute(), "external identity"));

                return ResultFromExtension<IUser>.Failed(string.Format(failMessageTemplate, "external identity"));
            }

            if (string.IsNullOrWhiteSpace(userPrincipalName))
            {
                log.Error($"We couldn't find a valid user principal name to use for the LDAP user '{displayName}' with email address '{emailAddress}'."
                          + string.Format(attributeErrorTemplate, configurationStore.GetUserPrincipalNameAttribute(), "user principal name"));

                return ResultFromExtension<IUser>.Failed(string.Format(failMessageTemplate, "user principal name"));
            }

            var authenticatingIdentity = identityCreator.Create(emailAddress, userPrincipalName, externalIdentity, displayName);
            var users = userStore.GetByIdentity(authenticatingIdentity);

            var existingMatchingUser = users.SingleOrDefault(u => u.Identities != null && u.Identities.Any(identity =>
                identity.IdentityProviderName == LdapAuthentication.ProviderName &&
                identity.Equals(authenticatingIdentity)));

            // if we can find a user where all identifiers match exactly then we know for sure that's the user who just logged in.
            if (existingMatchingUser != null)
                return ResultFromExtension<IUser>.Success(existingMatchingUser);

            foreach (var user in users)
            {
                // if we haven't converted the old externalId into the new identity then set it up now
                var anyLdapIdentity = user.Identities.FirstOrDefault(p => p.IdentityProviderName == LdapAuthentication.ProviderName);
                if (anyLdapIdentity == null)
                    return ResultFromExtension<IUser>.Success(userStore.AddIdentity(user.Id, authenticatingIdentity, cancellationToken));

                foreach (var identity in user.Identities.Where(p => p.IdentityProviderName == LdapAuthentication.ProviderName))
                {
                    if (identity.Claims[IdentityCreator.ExternalIdentityClaimType].Value == externalIdentity ||
                        identity.Claims[IdentityCreator.UpnClaimType].Value == userPrincipalName)
                    {
                        // if we partially matched but the samAccountName or UPN is the same then this is the same user.
                        identity.Claims[IdentityCreator.UpnClaimType].Value = userPrincipalName;
                        identity.Claims[ClaimDescriptor.EmailClaimType].Value = emailAddress;
                        identity.Claims[IdentityCreator.ExternalIdentityClaimType].Value = externalIdentity;
                        identity.Claims[ClaimDescriptor.DisplayNameClaimType].Value = displayName;

                        return ResultFromExtension<IUser>.Success(userStore.UpdateIdentity(user.Id, identity, cancellationToken));
                    }
                    else
                    {
                        // we found a single other user in our DB that wasn't an exact match, but matched on some fields, so see if that user is still in ldap
                        var otherUserPrincipal = ldapService.FindByIdentity(identity.Claims[IdentityCreator.ExternalIdentityClaimType].Value);

                        if (!otherUserPrincipal.Success)
                        {
                            // we couldn't find a match for the existing DB user's SamAccountName in ldap, assume their details have been updated in ldap
                            // and we need to modify the existing user in our DB.
                            identity.Claims[ClaimDescriptor.EmailClaimType].Value = emailAddress;
                            identity.Claims[IdentityCreator.UpnClaimType].Value = userPrincipalName;
                            identity.Claims[IdentityCreator.ExternalIdentityClaimType].Value = externalIdentity;
                            identity.Claims[ClaimDescriptor.DisplayNameClaimType].Value = displayName;

                            return ResultFromExtension<IUser>.Success(userStore.UpdateIdentity(user.Id, identity, cancellationToken));
                        }

                        // otherUserPrincipal still exists in ldap, so what we have here is a new user
                    }
                }
            }

            return CreateNewUser(cancellationToken, principal, authenticatingIdentity);
        }

        IResultFromExtension<IUser> CreateNewUser(CancellationToken cancellationToken, UserValidationResult principal, Identity authenticatingIdentity)
        {
            if (!configurationStore.GetAllowAutoUserCreation())
                return ResultFromExtension<IUser>.Failed("User could not be located, and auto user creation is not enabled.");

            var userCreateResult = userStore.Create(
                principal.UserPrincipalName,
                principal.DisplayName,
                principal.EmailAddress,
                cancellationToken,
                identities: new[] {authenticatingIdentity});

            if (userCreateResult is FailureResult failure)
                throw new ApplicationException($"Error creating user. {failure.ErrorString}");

            var successResult = (Result<IUser>) userCreateResult;
            return ResultFromExtension<IUser>.Success(successResult.Value);
        }
    }
}