using Octopus.Server.Extensibility.Extensions.Infrastructure.Web.Api;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Web
{
    public class UserLookupAction : IAsyncApiAction
    {
        private readonly ICanSearchLdapUsers userSearch;

        public UserLookupAction(
            ICanSearchLdapUsers userSearch)
        {
            this.userSearch = userSearch;
        }

        public Task ExecuteAsync(OctoContext context)
        {
            var name = context.Request.Query["partialName"]?.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(name))
            {
                context.Response.BadRequest("Please provide the name of a user to search for");
                return Task.FromResult(0);
            }

            using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1)))
            {
                context.Response.AsOctopusJson(userSearch.Search(name, cts.Token));
            }

            return Task.FromResult(0);
        }
    }
}