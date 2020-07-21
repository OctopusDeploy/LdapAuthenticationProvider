using Octopus.Data;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Web
{
    public class UserLookupAction : IAsyncApiAction
    {
        private static readonly IRequiredParameter<string> PartialName = new RequiredQueryParameterProperty<string>("partialName", "Partial username to lookup");
        private static readonly BadRequestRegistration Disabled = new BadRequestRegistration($"The {LdapAuthentication.ProviderName} is currently disabled");
        private static readonly OctopusJsonRegistration<ExternalUserLookupResult> SearchResults = new OctopusJsonRegistration<ExternalUserLookupResult>();

        private readonly ICanSearchLdapUsers userSearch;

        public UserLookupAction(
            ICanSearchLdapUsers userSearch)
        {
            this.userSearch = userSearch;
        }

        public Task<IOctoResponseProvider> ExecuteAsync(IOctoRequest request)
        {
            return request
                .HandleAsync(PartialName, name =>
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1)))
                    {
                        var externalUserLookupResult = userSearch.Search(name, cts.Token);
                        if (externalUserLookupResult is ISuccessResult<ExternalUserLookupResult> successResult)
                            return Task.FromResult(SearchResults.Response(successResult.Value));
                        return Task.FromResult(Disabled.Response());
                    }
                });
        }
    }
}