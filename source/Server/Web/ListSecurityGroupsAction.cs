using Octopus.Server.Extensibility.Authentication.HostServices;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Web
{
    public class ListSecurityGroupsAction : IAsyncApiAction
    {
        private readonly ILdapExternalSecurityGroupLocator externalSecurityGroupLocator;

        public ListSecurityGroupsAction(
            ILdapExternalSecurityGroupLocator externalSecurityGroupLocator)
        {
            this.externalSecurityGroupLocator = externalSecurityGroupLocator;
        }

        public Task ExecuteAsync(OctoContext context)
        {
            var name = context.Request.Query["partialName"]?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(name))
            {
                context.Response.BadRequest("Please provide the name of a group to search by, or a team");
                return Task.FromResult(0);
            }

            using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1)))
            {
                context.Response.AsOctopusJson(SearchByName(name, cts.Token));
            }

            return Task.FromResult(0);
        }

        private ExternalSecurityGroup[] SearchByName(string name, CancellationToken cancellationToken)
        {
            return externalSecurityGroupLocator.Search(name, cancellationToken).Groups;
        }
    }
}