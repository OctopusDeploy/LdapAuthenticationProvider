using Octopus.Data;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.HostServices;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Web
{
    public class ListSecurityGroupsAction : IAsyncApiAction
    {
        private static readonly IRequiredParameter<string> PartialName = new RequiredQueryParameterProperty<string>("partialName", "Partial group name to lookup");
        private static readonly BadRequestRegistration Disabled = new BadRequestRegistration($"The {LdapAuthentication.ProviderName} is currently disabled");
        private static readonly OctopusJsonRegistration<ExternalSecurityGroup[]> SearchResults = new OctopusJsonRegistration<ExternalSecurityGroup[]>();

        private readonly ILdapExternalSecurityGroupLocator externalSecurityGroupLocator;

        public ListSecurityGroupsAction(
            ILdapExternalSecurityGroupLocator externalSecurityGroupLocator)
        {
            this.externalSecurityGroupLocator = externalSecurityGroupLocator;
        }

        public Task<IOctoResponseProvider> ExecuteAsync(IOctoRequest request)
        {
            return request
                .HandleAsync(PartialName, name =>
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1)))
                    {
                        var result = externalSecurityGroupLocator.Search(name, cts.Token);
                        if (result is ISuccessResult<ExternalSecurityGroupResult> successResult)
                            return Task.FromResult(SearchResults.Response(successResult.Value.Groups));
                        return Task.FromResult(Disabled.Response());
                    }
                });
        }
    }
}