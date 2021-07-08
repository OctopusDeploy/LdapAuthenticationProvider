using Octopus.Data.Model.User;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;
using Octopus.Server.Extensibility.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class GroupRetriever : IExternalGroupRetriever
    {
        private readonly ISystemLog log;
        private readonly ILdapConfigurationStore configurationStore;
        private readonly ILdapExternalSecurityGroupLocator groupLocator;

        public GroupRetriever(
            ISystemLog log,
            ILdapConfigurationStore configurationStore,
            ILdapExternalSecurityGroupLocator groupLocator)
        {
            this.log = log;
            this.configurationStore = configurationStore;
            this.groupLocator = groupLocator;
        }

        public string IdentityProviderName => LdapAuthentication.ProviderName;

        public IResultFromExtension<ExternalGroupResult> Read(IUser user, CancellationToken cancellationToken)
        {
            if (!configurationStore.GetIsEnabled())
                return ResultFromExtension<ExternalGroupResult>.ExtensionDisabled();
            if (user.Username == User.GuestLogin)
                return ResultFromExtension<ExternalGroupResult>.Failed("Not valid for Guest user");
            if (user.Identities.All(p => p.IdentityProviderName != LdapAuthentication.ProviderName))
                return ResultFromExtension<ExternalGroupResult>.Failed("No identities matching this provider");

            // if the user has multiple, unique identities assigned then the group list should be the distinct union of the groups from
            // all of the identities
            var wasAbleToRetrieveSomeGroups = false;
            var newGroups = new HashSet<GroupDistinguishedName>();
            var ldapIdentities = user.Identities.Where(p => p.IdentityProviderName == LdapAuthentication.ProviderName);
            foreach (var ldapIdentity in ldapIdentities)
            {
                var uniqueAccountName = ldapIdentity.Claims[IdentityCreator.UniqueAccountNameClaimType].Value;

                if (string.IsNullOrEmpty(uniqueAccountName))
                {
                    log.WarnFormat("Unable to retrieve groups due to invalid stored identity for user '{0}'", user.Username);
                    continue;
                }

                var result = groupLocator.GetGroupIdsForUser(uniqueAccountName, cancellationToken);
                if (result.WasAbleToRetrieveGroups)
                {
                    foreach (var groupId in result.GroupDistinguishedNames.Where(g => !newGroups.Contains(g)))
                    {
                        newGroups.Add(groupId);
                    }
                    wasAbleToRetrieveSomeGroups = true;
                }
                else
                {
                    log.WarnFormat("Unable to retrieve groups for unique account name '{0}'", uniqueAccountName);
                }
            }

            if (!wasAbleToRetrieveSomeGroups)
            {
                log.ErrorFormat("Unable to retrieve groups for user {0}", user.Username);
                return ResultFromExtension<ExternalGroupResult>.Failed($"Unable to retrieve groups for user {user.Username}");
            }

            var groupsDNs = newGroups.Select(g => g.ToString());
            return ResultFromExtension<ExternalGroupResult>.Success(new ExternalGroupResult(groupsDNs));
        }
    }
}