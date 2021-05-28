using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;
using Octopus.Server.Extensibility.Results;
using System.Linq;
using System.Threading;
using Octopus.Server.Extensibility.Authentication.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserSearch : ICanSearchLdapUsers
    {
        private readonly ILdapContextProvider contextProvider;
        private readonly ILdapObjectNameNormalizer objectNameNormalizer;
        private readonly ILdapConfigurationStore configurationStore;
        private readonly IUserPrincipalFinder userPrincipalFinder;
        private readonly IIdentityCreator identityCreator;

        public string IdentityProviderName => LdapAuthentication.ProviderName;

        public UserSearch(
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

        public IResultFromExtension<ExternalUserLookupResult> Search(string searchTerm, CancellationToken cancellationToken)
        {
            if (!configurationStore.GetIsEnabled())
                return ResultFromExtension<ExternalUserLookupResult>.ExtensionDisabled();

            objectNameNormalizer.NormalizeName(searchTerm, out var partialName, out var domain);

            using (var context = contextProvider.GetContext())
            {
                if (cancellationToken.IsCancellationRequested) return null;

                var identities = userPrincipalFinder.SearchUser(context, searchTerm);

                var identityResources = identities.Distinct(new UserPrincipalComparer())
                    .Select(u => identityCreator.Create(u.Mail, u.UPN, u.SamAccountName, u.DisplayName).ToResource())
                    .ToArray();

                return ResultFromExtension<ExternalUserLookupResult>.Success(new ExternalUserLookupResult(identityResources));
            }
        }
    }

    public interface ICanSearchLdapUsers : ICanSearchExternalUsers { }
}