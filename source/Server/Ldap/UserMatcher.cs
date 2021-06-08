using Octopus.Data.Model.User;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;
using System.Threading;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserMatcher : ICanMatchExternalUser
    {
        private readonly ILdapContextProvider contextProvider;
        private readonly ILdapObjectNameNormalizer objectNameNormalizer;
        private readonly ILdapConfigurationStore configurationStore;
        private readonly IUserPrincipalFinder userPrincipalFinder;
        private readonly IIdentityCreator identityCreator;

        public string IdentityProviderName => LdapAuthentication.ProviderName;

        public UserMatcher(
            ILdapContextProvider contextProvider,
            ILdapObjectNameNormalizer objectNameNormalizer,
            ILdapConfigurationStore configurationStore,
            IUserPrincipalFinder userPrincipalFinder,
            IIdentityCreator identityCreator)
        {
            this.contextProvider = contextProvider;
            this.objectNameNormalizer = objectNameNormalizer;
            this.configurationStore = configurationStore;
            this.userPrincipalFinder = userPrincipalFinder;
            this.identityCreator = identityCreator;
        }

        public Identity Match(string name, CancellationToken cancellationToken)
        {
            if (!configurationStore.GetIsEnabled())
                return null;

            objectNameNormalizer.NormalizeName(name, out var normalisedName, out var domain);

            using var context = contextProvider.GetContext();
            if (cancellationToken.IsCancellationRequested) return null;

            var identityName = objectNameNormalizer.BuildUserName(normalisedName, domain);
            var match = userPrincipalFinder.FindByIdentity(context, identityName);

            if (match == null)
                return null;

            return identityCreator.Create(match.Mail, match.UPN, match.ExternalIdentity, match.DisplayName);
        }
    }
}