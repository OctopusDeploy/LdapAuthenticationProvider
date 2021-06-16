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
            log.Verbose($"Validating credentials provided for '{username}'...");

            return GetOrCreateCommon(username, cancellationToken, () => ldapService.ValidateCredentials(username, password, cancellationToken));
        }

        public IResultFromExtension<IUser> GetOrCreateUser(string username, CancellationToken cancellationToken)
        {
            return GetOrCreateCommon(username, cancellationToken, () => ldapService.FindByIdentity(username));
        }

        IResultFromExtension<IUser> GetOrCreateCommon(string username, CancellationToken cancellationToken, Func<UserValidationResult> ldapFunction)
        {
            if (!configurationStore.GetIsEnabled())
                return ResultFromExtension<IUser>.ExtensionDisabled();

            if (string.IsNullOrWhiteSpace(username))
                return ResultFromExtension<IUser>.Failed("No username provided");

            try
            {
                var userValidationResult = ldapFunction();

                return string.IsNullOrWhiteSpace(userValidationResult.ValidationMessage)
                    ? GetOrCreateUser(userValidationResult, cancellationToken)
                    : ResultFromExtension<IUser>.Failed(userValidationResult.ValidationMessage);
            }
            catch (LdapAuthenticationException ex)
            {
                log.Error(ex, ex.Message);
                return ResultFromExtension<IUser>.Failed(ex.Message);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Unable to find or validate credentials with LDAP provider.");
                return ResultFromExtension<IUser>.Failed("Unable to find or validate credentials with LDAP provider.");
            }
        }

        IResultFromExtension<IUser> GetOrCreateUser(UserValidationResult principal, CancellationToken cancellationToken)
        {
            var uniqueAccountName = principal.UniqueAccountName;
            var displayName = principal.DisplayName;
            var emailAddress = principal.EmailAddress;
            var userPrincipalName = principal.UserPrincipalName;

            const string attributeErrorTemplate = "Octopus is configured to use the '{0}' attribute as the {1} for LDAP users. " +
                                                  "Please make sure this user has a valid '{0}' attribute.";
            const string failMessageTemplate = "We were unable to find a valid {0} when attempting to sign in with LDAP. Please contact your administrator to resolve this.";

            if (string.IsNullOrWhiteSpace(principal.UniqueAccountName))
            {
                log.Error($"We couldn't find a valid unique account name to use for the LDAP user '{principal.DisplayName}' with email address '{principal.EmailAddress}' for the user account named '{principal.UserPrincipalName}'. "
                          + string.Format(attributeErrorTemplate, configurationStore.GetUniqueAccountNameAttribute(), "unique account name"));

                return ResultFromExtension<IUser>.Failed(string.Format(failMessageTemplate, "unique account name"));
            }

            if (string.IsNullOrWhiteSpace(userPrincipalName))
            {
                log.Error($"We couldn't find a valid user principal name to use for the LDAP user '{principal.DisplayName}' with email address '{principal.EmailAddress}'."
                          + string.Format(attributeErrorTemplate, configurationStore.GetUserPrincipalNameAttribute(), "user principal name"));

                return ResultFromExtension<IUser>.Failed(string.Format(failMessageTemplate, "user principal name"));
            }

            var authenticatingIdentity = identityCreator.Create(emailAddress, userPrincipalName, uniqueAccountName, displayName);
            var users = userStore.GetByIdentity(authenticatingIdentity);

            var existingMatchingUser = users.SingleOrDefault(u => u.Identities != null && u.Identities.Any(identity =>
                identity.IdentityProviderName == LdapAuthentication.ProviderName &&
                identity.Equals(authenticatingIdentity)));

            // if we can find a user where all identifiers match exactly then we know for sure that's the user who just logged in.
            if (existingMatchingUser != null)
                return ResultFromExtension<IUser>.Success(existingMatchingUser);

            foreach (var user in users)
            {
                // if we haven't converted the old unique account name into the new identity then set it up now
                var anyLdapIdentity = user.Identities.FirstOrDefault(p => p.IdentityProviderName == LdapAuthentication.ProviderName);
                if (anyLdapIdentity == null)
                    return ResultFromExtension<IUser>.Success(userStore.AddIdentity(user.Id, authenticatingIdentity, cancellationToken));

                foreach (var identity in user.Identities.Where(p => p.IdentityProviderName == LdapAuthentication.ProviderName))
                {
                    // if we partially matched but the uniqueAccountName or UPN is the same then this is the same user, so we will need to update user in our DB.
                    var identityMatchesSameUser = identity.Claims[IdentityCreator.UniqueAccountNameClaimType].Value == uniqueAccountName ||
                                                      identity.Claims[IdentityCreator.UserPrincipleNameClaimType].Value == userPrincipalName;

                    if (!identityMatchesSameUser)
                    {
                        // we found a single other user in our DB that wasn't an exact match, but matched on some fields, so see if that user is still in ldap
                        var otherUserPrincipal = ldapService.FindByIdentity(identity.Claims[IdentityCreator.UniqueAccountNameClaimType].Value);

                        // otherUserPrincipal still exists in ldap, so what we have here is probably a new user, but we need to keep checking.
                        if (otherUserPrincipal.Success)
                            continue;

                        // we couldn't find a match for the existing DB user's uniqueAccountName in ldap, assume their details have been updated in ldap
                        // and we need to modify the existing user in our DB.
                    }

                    SetClaimValuesOnIdentity(identity, emailAddress, userPrincipalName, uniqueAccountName, displayName);
                    var updatedUser = userStore.UpdateIdentity(user.Id, identity, cancellationToken);

                    return ResultFromExtension<IUser>.Success(updatedUser);
                }
            }

            return CreateNewUser(cancellationToken, principal, authenticatingIdentity);
        }

        void SetClaimValuesOnIdentity(Identity identity, string emailAddress, string userPrincipalName, string uniqueAccountName, string displayName)
        {
            identity.Claims[ClaimDescriptor.EmailClaimType].Value = emailAddress;
            identity.Claims[IdentityCreator.UserPrincipleNameClaimType].Value = userPrincipalName;
            identity.Claims[IdentityCreator.UniqueAccountNameClaimType].Value = uniqueAccountName;
            identity.Claims[ClaimDescriptor.DisplayNameClaimType].Value = displayName;
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