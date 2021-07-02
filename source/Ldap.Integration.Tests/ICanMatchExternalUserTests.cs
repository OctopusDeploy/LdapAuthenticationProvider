using System.Threading;
using Ldap.Integration.Tests.TestHelpers;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Xunit;

namespace Ldap.Integration.Tests
{
    public class ICanMatchExternalUserTests
    {
        public class TheMatchMethod
        {
            [Fact]
            internal void MatchesAUserFromActiveDirectory()
            {
                var userName = "developer1";
                var expectedUpn = "developer1@mycompany.local";
                var expectedUan = "developer1";
                var expectedEmail = "developer1@mycompany.local";
                var expectedDisplayName = "Developer User 1";

                ICanMatchExternalUser fixture = FixtureHelper.CreateUserMatcher(ConfigurationHelper.GetActiveDirectoryConfiguration());

                var match = fixture.Match(userName, new CancellationToken());

                Assert.NotNull(match);
                Assert.Equal(expectedUan, match.Claims["uan"].Value);
                Assert.Equal(expectedUpn, match.Claims["upn"].Value);
                Assert.Equal(expectedEmail, match.Claims["email"].Value);
                Assert.Equal(expectedDisplayName, match.Claims["dn"].Value);
            }

            [Fact]
            internal void MatchesAUserFromOpenLDAP()
            {
                var userName = "developer1";
                var expectedUpn = "developer1";
                var expectedUan = "developer1";
                var expectedEmail = "developer1@gmail.com";
                var expectedDisplayName = "Developer User 1";

                ICanMatchExternalUser fixture = FixtureHelper.CreateUserMatcher(ConfigurationHelper.GetOpenLdapConfiguration());

                var match = fixture.Match(userName, new CancellationToken());

                Assert.NotNull(match);
                Assert.Equal(expectedUan, match.Claims["uan"].Value);
                Assert.Equal(expectedUpn, match.Claims["upn"].Value);
                Assert.Equal(expectedEmail, match.Claims["email"].Value);
                Assert.Equal(expectedDisplayName, match.Claims["dn"].Value);
            }
        }
    }
}