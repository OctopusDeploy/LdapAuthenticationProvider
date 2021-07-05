using System.Linq;
using System.Threading;
using Ldap.Integration.Tests.TestHelpers;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Results;
using Xunit;

namespace Ldap.Integration.Tests
{
    public class ICanSearchExternalUsersTests
    {
        public class TheSearchMethod
        {
            [Fact]
            internal void FindsUsersFromActiveDirectory()
            {
                var partialName = "devel";

                ICanSearchExternalUsers fixture = FixtureHelper.CreateUserSearch(ConfigurationHelper.GetActiveDirectoryConfiguration());

                var result = fixture.Search(partialName, new CancellationToken());

                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var searchResult = ((ResultFromExtension<ExternalUserLookupResult>)result).Value;

                Assert.Equal(2, searchResult.Identities.Length);
                Assert.Contains(searchResult.Identities, x => x.Claims["uan"].Value == "developer1");
                Assert.Contains(searchResult.Identities, x => x.Claims["uan"].Value == "developer2");
            }

            [Fact]
            internal void FindsUsersFromOpenLDAP()
            {
                var partialName = "devel";
                
                ICanSearchExternalUsers fixture = FixtureHelper.CreateUserSearch(ConfigurationHelper.GetOpenLdapConfiguration());

                var result = fixture.Search(partialName, new CancellationToken());

                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var searchResult = ((ResultFromExtension<ExternalUserLookupResult>)result).Value;

                Assert.Equal(2, searchResult.Identities.Length);
                Assert.Contains(searchResult.Identities, x => x.Claims["uan"].Value == "developer1");
                Assert.Contains(searchResult.Identities, x => x.Claims["uan"].Value == "developer2");
            }
        }
    }
}