using System.Linq;
using Ldap.Integration.Tests.TestHelpers;
using Octopus.Server.Extensibility.Authentication.Ldap;
using Xunit;

namespace Ldap.Integration.Tests
{
    public class TheUserPrincipalFinder
    {
        [Fact]
        internal void FindsAUserFromActiveDirectory()
        {
            var userName = "developer1";
            var expectedDistinguishedName = "cn=developer1,cn=Users,dc=mycompany,dc=local";
            var expectedUpn = "developer1@mycompany.local";
            var expectedUan = "developer1";
            var expectedEmail = "developer1@mycompany.local";
            var expectedDisplayName = "Developer User 1";
            var expectedGroups = new[]
            {
                "cn=DeveloperGroup1,ou=Groups,dc=mycompany,dc=local"
            };

            var contextFixture = FixtureHelper.CreateLdapContext(ConfigurationHelper.GetActiveDirectoryConfiguration());

            var user = new UserPrincipalFinder().FindByIdentity(contextFixture, userName);

            Assert.NotNull(user);
            Assert.Equal(expectedDistinguishedName, user.DistinguishedName, true);
            Assert.Equal(expectedUan, user.UniqueAccountName, true);
            Assert.Equal(expectedUpn, user.UserPrincipalName, true);
            Assert.Equal(expectedEmail, user.Email, true);
            Assert.Equal(expectedDisplayName, user.DisplayName, true);
            Assert.Equal(expectedGroups.Select(x => x.ToLowerInvariant()).OrderBy(x => x), user.Groups.Select(x => x.Value.ToLowerInvariant()).OrderBy(x => x));
        }

        [Fact]
        internal void FindsAUserFromOpenLDAP()
        {
            var userName = "developer1";
            var expectedDistinguishedName = "cn=developer1,dc=domain1,dc=local";
            var expectedUpn = "developer1";
            var expectedUan = "developer1";
            var expectedEmail = "developer1@gmail.com";
            var expectedDisplayName = "Developer User 1";
            var expectedGroups = new[]
            {
                "cn=DeveloperGroup1,ou=Groups,dc=domain1,dc=local"
            };

            var contextFixture = FixtureHelper.CreateLdapContext(ConfigurationHelper.GetOpenLdapConfiguration());

            var user = new UserPrincipalFinder().FindByIdentity(contextFixture, userName);

            Assert.NotNull(user);
            Assert.Equal(expectedDistinguishedName, user.DistinguishedName, true);
            Assert.Equal(expectedUan, user.UniqueAccountName, true);
            Assert.Equal(expectedUpn, user.UserPrincipalName, true);
            Assert.Equal(expectedEmail, user.Email, true);
            Assert.Equal(expectedDisplayName, user.DisplayName, true);
            Assert.Equal(expectedGroups.Select(x => x.ToLowerInvariant()).OrderBy(x => x), user.Groups.Select(x => x.Value.ToLowerInvariant()).OrderBy(x => x));
        }
    }
}