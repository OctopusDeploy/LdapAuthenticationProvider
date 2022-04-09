using System.Threading;
using Ldap.Integration.Tests.TestHelpers;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Ldap.Integration.Tests
{
    public class ICanMatchExternalUserTests
    {
        public class TheMatchMethod
        {
            readonly ITestOutputHelper _testLogger;
            public TheMatchMethod(ITestOutputHelper testLogger)
            {
                _testLogger = testLogger;
            }

            [Fact]
            internal void MatchesAUserFromActiveDirectory()
            {
                var userName = "developer1";
                var expectedUpn = "developer1@mycompany.local";
                var expectedUan = "developer1";
                var expectedEmail = "developer1@mycompany.local";
                var expectedDisplayName = "Developer User 1";

                ICanMatchExternalUser fixture = FixtureHelper.CreateUserMatcher(ConfigurationHelper.GetActiveDirectoryConfiguration(), _testLogger);

                var match = fixture.Match(userName, new CancellationToken());

                Assert.NotNull(match);
                Assert.Equal(expectedUan, match.Claims["uan"].Value);
                Assert.Equal(expectedUpn, match.Claims["upn"].Value);
                Assert.Equal(expectedEmail, match.Claims["email"].Value);
                Assert.Equal(expectedDisplayName, match.Claims["dn"].Value);
            }

            [Fact]
            [Trait("AuthProvider","OpenLDAP")]
            internal void MatchesAUserFromOpenLDAP()
            {
                var userName = "developer1";
                var expectedUpn = "developer1";
                var expectedUan = "developer1";
                var expectedEmail = "developer1@gmail.com";
                var expectedDisplayName = "Developer User 1";

                ICanMatchExternalUser fixture = FixtureHelper.CreateUserMatcher(ConfigurationHelper.GetOpenLdapConfiguration(), _testLogger);

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