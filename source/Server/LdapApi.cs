using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Web;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using System;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapApi : RegisterEndpoint
    {
        public const string ApiExternalGroupsSearch = "/api/externalgroups/ldap{?partialName}";
        public const string ApiExternalUsersSearch = "/api/externalusers/ldap{?partialName}";

        public LdapApi(
            Func<SecuredWhenEnabledAsyncActionInvoker<ListSecurityGroupsAction, ILdapConfigurationStore>> listSecurityGroupsActionInvokerFactory,
            Func<SecuredWhenEnabledAsyncActionInvoker<UserLookupAction, ILdapConfigurationStore>> userLookupActionInvokerFactory)
        {
            Add("GET", ApiExternalGroupsSearch, listSecurityGroupsActionInvokerFactory().ExecuteAsync);
            Add("GET", ApiExternalUsersSearch, userLookupActionInvokerFactory().ExecuteAsync);
        }
    }
}