using System.Threading;
using Ldap.Integration.Tests.TestHelpers;
using Xunit;

namespace Ldap.Integration.Tests
{
    public class TheUserMatcher
    {
        [Fact]
        internal void MatchesAUserFromActiveDirectory()
        {
            var userName = "developer1";
            var expectedUpn = "developer1@mycompany.local";
            var expectedUan = "developer1";
            var expectedEmail = "developer1@mycompany.local";
            var expectedDisplayName = "Developer User 1";

            var fixture = FixtureHelper.CreateUserMatcher(ConfigurationHelper.GetActiveDirectoryConfiguration());

            var match = fixture.Match(userName, new CancellationToken());

            Assert.NotNull(match);
            Assert.Equal(expectedUan, match.Claims["uan"].Value);
            Assert.Equal(expectedUpn, match.Claims["upn"].Value);
            Assert.Equal(expectedEmail, match.Claims["email"].Value);
            Assert.Equal(expectedDisplayName, match.Claims["dn"].Value);
        }

        [Fact]
        internal void MatchesAUserAUserFromOpenLDAP()
        {
            var userName = "developer1";
            var expectedUpn = "developer1";
            var expectedUan = "developer1";
            var expectedEmail = "developer1@gmail.com";
            var expectedDisplayName = "Developer User 1";

            var fixture = FixtureHelper.CreateUserMatcher(ConfigurationHelper.GetOpenLdapConfiguration());

            var match = fixture.Match(userName, new CancellationToken());

            Assert.NotNull(match);
            Assert.Equal(expectedUan, match.Claims["uan"].Value);
            Assert.Equal(expectedUpn, match.Claims["upn"].Value);
            Assert.Equal(expectedEmail, match.Claims["email"].Value);
            Assert.Equal(expectedDisplayName, match.Claims["dn"].Value);
        }
    }
}