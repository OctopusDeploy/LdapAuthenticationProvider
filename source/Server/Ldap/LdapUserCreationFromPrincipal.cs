using Octopus.Data.Model.User;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Results;
using System.Security.Principal;
using System.Threading;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapUserCreationFromPrincipal : ISupportsAutoUserCreationFromPrincipal
    {
        private readonly ILdapConfigurationStore configurationStore;
        private readonly ILdapCredentialValidator credentialValidator;

        public string IdentityProviderName => LdapAuthentication.ProviderName;

        public LdapUserCreationFromPrincipal(
            ILdapConfigurationStore configurationStore,
            ILdapCredentialValidator credentialValidator)
        {
            this.configurationStore = configurationStore;
            this.credentialValidator = credentialValidator;
        }

        public IResultFromExtension<IUser> GetOrCreateUser(IPrincipal principal, CancellationToken cancellationToken)
        {
            return !configurationStore.GetIsEnabled() ?
                ResultFromExtension<IUser>.ExtensionDisabled() :
                credentialValidator.GetOrCreateUser(principal.Identity.Name, cancellationToken);
        }
    }
}