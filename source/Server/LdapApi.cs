using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Web;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapApi : RegistersEndpoints
    {
        public const string ApiExternalGroupsSearch = "/api/externalgroups/ldap{?partialName}";
        public const string ApiExternalUsersSearch = "/api/externalusers/ldap{?partialName}";

        public LdapApi()
        {
            Add<ListSecurityGroupsAction>("GET", ApiExternalGroupsSearch, RouteCategory.Raw, new SecuredWhenEnabledEndpointInvocation<ILdapConfigurationStore>(), "Search for LDAP groups", "LDAP");
            Add<UserLookupAction>("GET", ApiExternalUsersSearch, RouteCategory.Raw, new SecuredWhenEnabledEndpointInvocation<ILdapConfigurationStore>(), "Search for LDAP users", "LDAP");
        }
    }
}