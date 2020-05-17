using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Storage.User;
using System.Security.Principal;
using System.Threading;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapUserCreationFromPrincipal : ISupportsAutoUserCreationFromPrincipal
    {
        private readonly ILdapConfigurationStore configurationStore;
        private readonly ILdapCredentialValidator credentialValidator;

        public LdapUserCreationFromPrincipal(
            ILdapConfigurationStore configurationStore,
            ILdapCredentialValidator credentialValidator)
        {
            this.configurationStore = configurationStore;
            this.credentialValidator = credentialValidator;
        }

        public AuthenticationUserCreateResult GetOrCreateUser(IPrincipal principal, CancellationToken cancellationToken)
        {
            return !configurationStore.GetIsEnabled() ?
                new AuthenticationUserCreateResult() :
                credentialValidator.GetOrCreateUser(principal.Identity.Name, cancellationToken);
        }
    }
}