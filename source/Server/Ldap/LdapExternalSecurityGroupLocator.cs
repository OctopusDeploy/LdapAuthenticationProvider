using Novell.Directory.Ldap;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.HostServices;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapExternalSecurityGroupLocator : ILdapExternalSecurityGroupLocator
    {
        readonly ISystemLog log;
        readonly ILdapContextProvider contextProvider;
        readonly ILdapObjectNameNormalizer objectNameNormalizer;
        readonly ILdapConfigurationStore configurationStore;
        readonly IUserPrincipalFinder userPrincipalFinder;
        readonly INestedGroupFinder nestedGroupFinder;

        public string IdentityProviderName => LdapAuthentication.ProviderName;

        public LdapExternalSecurityGroupLocator(
            ISystemLog log,
            ILdapContextProvider contextProvider,
            ILdapObjectNameNormalizer objectNameNormalizer,
            ILdapConfigurationStore configurationStore,
            IUserPrincipalFinder userPrincipalFinder,
            INestedGroupFinder nestedGroupFinder)
        {
            this.log = log;
            this.contextProvider = contextProvider;
            this.objectNameNormalizer = objectNameNormalizer;
            this.configurationStore = configurationStore;
            this.userPrincipalFinder = userPrincipalFinder;
            this.nestedGroupFinder = nestedGroupFinder;
        }

        public IResultFromExtension<ExternalSecurityGroupResult> Search(string name, CancellationToken cancellationToken)
        {
            if (!configurationStore.GetIsEnabled())
                return ResultFromExtension<ExternalSecurityGroupResult>.ExtensionDisabled();

            var groups = FindGroups(name, cancellationToken);
            var result = new ExternalSecurityGroupResult(groups);

            return ResultFromExtension<ExternalSecurityGroupResult>.Success(result);
        }

        public ExternalSecurityGroup[] FindGroups(string name, CancellationToken cancellationToken)
        {
            var results = new List<ExternalSecurityGroup>();
            objectNameNormalizer.NormalizeName(name, out string partialGroupName, out string? _);
            using var context = contextProvider.GetContext();
            var filterToken = $"*{partialGroupName}*";
            var lsc = context.LdapConnection.Search(
                context.GroupBaseDN,
                LdapConnection.ScopeSub,
                context.GroupFilter?.Replace("*", filterToken),
                new[] { context.GroupNameAttribute },
                false
            );
            var searchResults = lsc.ToList();
            results.AddRange(searchResults.Select(x => new ExternalSecurityGroup
            {
                Id = x.Dn,
                DisplayName = x.GetAttribute(context.GroupNameAttribute).StringValue
            }));

            return results.OrderBy(o => o.DisplayName).ToArray();
        }

        public LdapExternalSecurityGroupLocatorResult GetGroupIdsForUser(string username, CancellationToken cancellationToken)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), "The username is null, indicating we were not able to associate this Octopus user account with an identifier from LDAP.");

            try
            {
                log.Verbose($"Finding external security groups for '{username}'...");

                objectNameNormalizer.NormalizeName(username, out username, out var domain);

                using var context = contextProvider.GetContext();
                cancellationToken.ThrowIfCancellationRequested();

                var uniqueAccountName = objectNameNormalizer.BuildUserName(username, domain);
                var principal = userPrincipalFinder.FindByIdentity(context, uniqueAccountName);

                if (principal == null)
                {
                    var searchedContext = domain ?? context.UserBaseDN;
                    log.Trace($"While loading security groups, a principal identifiable by '{uniqueAccountName}' was not found in '{searchedContext}'");
                    return new LdapExternalSecurityGroupLocatorResult();
                }

                var groups = nestedGroupFinder.FindAllParentGroups(context, configurationStore.GetNestedGroupSearchDepth(), principal.Groups);

                cancellationToken.ThrowIfCancellationRequested();

                return new LdapExternalSecurityGroupLocatorResult(groups);
            }
            catch (OperationCanceledException)
            {
                return new LdapExternalSecurityGroupLocatorResult();
            }
            catch (Exception ex)
            {
                log.ErrorFormat(ex, "LDAP search for {0} failed.", username);
                return new LdapExternalSecurityGroupLocatorResult();
            }
        }
    }
}