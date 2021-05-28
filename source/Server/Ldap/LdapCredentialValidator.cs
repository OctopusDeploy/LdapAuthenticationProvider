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

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapCredentialValidator : ILdapCredentialValidator
    {
        private readonly ISystemLog log;
        private readonly ILdapObjectNameNormalizer objectNameNormalizer;
        private readonly IUpdateableUserStore userStore;
        private readonly ILdapConfigurationStore configurationStore;
        private readonly IIdentityCreator identityCreator;
        private readonly ILdapService ldapService;

        internal static string EnvironmentUserDomainName = Environment.UserDomainName;

        public string IdentityProviderName => LdapAuthentication.ProviderName;

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

        public int Priority => 100;

        public IResultFromExtension<IUser> ValidateCredentials(string username, string password, CancellationToken cancellationToken)
        {
            if (!configurationStore.GetIsEnabled())
            {
                return ResultFromExtension<IUser>.ExtensionDisabled();
            }

            if (username == null) throw new ArgumentNullException(nameof(username));

            log.Verbose($"Validating credentials provided for '{username}'...");

            var validatedUser = ldapService.ValidateCredentials(username, password, cancellationToken);
            if (!string.IsNullOrWhiteSpace(validatedUser.ValidationMessage))
            {
                return ResultFromExtension<IUser>.Failed(validatedUser.ValidationMessage);
            }

            return GetOrCreateUser(validatedUser, cancellationToken);
        }

        public IResultFromExtension<IUser> GetOrCreateUser(string username, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(username))
                return ResultFromExtension<IUser>.Failed("No username provided");

            var result = ldapService.FindByIdentity(username);

            if (!string.IsNullOrWhiteSpace(result.ValidationMessage))
            {
                throw new ArgumentException(result.ValidationMessage);
            }

            return GetOrCreateUser(result, cancellationToken);
        }

        internal IResultFromExtension<IUser> GetOrCreateUser(UserValidationResult principal, CancellationToken cancellationToken)
        {
            var samAccountName = principal.SamAccountName;
            var displayName = principal.DisplayName;
            var emailAddress = principal.EmailAddress;
            var userPrincipalName = principal.UserPrincipalName;

            if (string.IsNullOrWhiteSpace(samAccountName))
            {
                log.Error($"We couldn't find a valid external identity to use for the LDAP user '{displayName}' with email address '{emailAddress}'.");
            }

            var authenticatingIdentity = identityCreator.Create(emailAddress, userPrincipalName, samAccountName, displayName);

            var users = userStore.GetByIdentity(authenticatingIdentity);

            var existingMatchingUser = users.SingleOrDefault(u => u.Identities != null && u.Identities.Any(identity =>
                identity.IdentityProviderName == LdapAuthentication.ProviderName &&
                identity.Equals(authenticatingIdentity)));

            // if we can find a user where all identifiers match exactly then we know for sure that's the user
            // who just logged in.
            if (existingMatchingUser != null)
            {
                return ResultFromExtension<IUser>.Success(existingMatchingUser);
            }

            foreach (var user in users)
            {
                // if we haven't converted the old externalId into the new identity then set it up now
                var anyLdapIdentity = user.Identities.FirstOrDefault(p => p.IdentityProviderName == LdapAuthentication.ProviderName);
                if (anyLdapIdentity == null)
                {
                    return ResultFromExtension<IUser>.Success(userStore.AddIdentity(user.Id, authenticatingIdentity, cancellationToken));
                }

                foreach (var identity in user.Identities.Where(p => p.IdentityProviderName == LdapAuthentication.ProviderName))
                {
                    if (identity.Claims[IdentityCreator.SamAccountNameClaimType].Value == samAccountName ||
                        identity.Claims[IdentityCreator.UpnClaimType].Value == userPrincipalName)
                    {
                        // if we partially matched but the samAccountName or UPN is the same then this is the same user.
                        identity.Claims[IdentityCreator.UpnClaimType].Value = userPrincipalName;
                        identity.Claims[ClaimDescriptor.EmailClaimType].Value = emailAddress;
                        identity.Claims[IdentityCreator.SamAccountNameClaimType].Value = samAccountName;
                        identity.Claims[ClaimDescriptor.DisplayNameClaimType].Value = displayName;

                        return ResultFromExtension<IUser>.Success(userStore.UpdateIdentity(user.Id, identity, cancellationToken));
                    }
                    else
                    {
                        // we found a single other user in our DB that wasn't an exact match, but matched on some fields, so see if that user is still
                        // in ldap
                        var otherUserPrincipal = ldapService.FindByIdentity(identity.Claims[IdentityCreator.SamAccountNameClaimType].Value);

                        if (!otherUserPrincipal.Success)
                        {
                            // we couldn't find a match for the existing DB user's SamAccountName in ldap, assume their details have been updated in ldap
                            // and we need to modify the existing user in our DB.
                            identity.Claims[ClaimDescriptor.EmailClaimType].Value = emailAddress;
                            identity.Claims[IdentityCreator.UpnClaimType].Value = userPrincipalName;
                            identity.Claims[IdentityCreator.SamAccountNameClaimType].Value = samAccountName;
                            identity.Claims[ClaimDescriptor.DisplayNameClaimType].Value = displayName;

                            return ResultFromExtension<IUser>.Success(userStore.UpdateIdentity(user.Id, identity, cancellationToken));
                        }

                        // otherUserPrincipal still exists in ldap, so what we have here is a new user
                    }
                }
            }

            var userCreateResult = userStore.Create(
                userPrincipalName,
                displayName,
                emailAddress,
                cancellationToken,
                identities: new[] { authenticatingIdentity });

            if (userCreateResult is FailureResult failure)
                throw new ApplicationException($"Error creating user. {failure.ErrorString}");

            var successResult = ((Result<IUser>)userCreateResult);
            return ResultFromExtension<IUser>.Success(successResult.Value);
        }
    }
}