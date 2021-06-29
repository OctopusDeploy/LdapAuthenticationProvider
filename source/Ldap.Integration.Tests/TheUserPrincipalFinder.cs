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
            var userName = "developerA";
            var expectedDistinguishedName = "cn=developer A,cn=Users,dc=mycompany,dc=local";
            var expectedGroups = new[]
            {
                "cn=DeveloperGroupA2,ou=Groups,dc=mycompany,dc=local"
            };

            var contextFixture = FixtureHelper.CreateLdapContext(ConfigurationHelper.GetActiveDirectoryConfiguration());

            var user = new UserPrincipalFinder().FindByIdentity(contextFixture, userName);

            Assert.NotNull(user);
            Assert.Equal(expectedDistinguishedName, user.DistinguishedName, true);
            Assert.Equal(expectedGroups.Length, user.Groups.Count());
            Assert.Equal(expectedGroups.Select(x => x.ToLowerInvariant()).OrderBy(x => x), user.Groups.Select(x => x.Value.ToLowerInvariant()).OrderBy(x => x));
        }

        [Fact]
        internal void FindsAUserFromOpenLDAP()
        {
            var userName = "developer";
            var expectedDistinguishedName = "cn=developer,dc=domain1,dc=local";
            var expectedGroups = new[]
            {
                "cn=SubMaintainers,cn=Maintaners,ou=Groups,dc=domain1,dc=local"
            };

            var contextFixture = FixtureHelper.CreateLdapContext(ConfigurationHelper.GetOpenLdapConfiguration());

            var user = new UserPrincipalFinder().FindByIdentity(contextFixture, userName);

            Assert.NotNull(user);
            Assert.Equal(expectedDistinguishedName, user.DistinguishedName, true);
            Assert.Equal(expectedGroups.Length, user.Groups.Count());
            Assert.Equal(expectedGroups.Select(x => x.ToLowerInvariant()).OrderBy(x => x), user.Groups.Select(x => x.Value.ToLowerInvariant()).OrderBy(x => x));
        }

    }
}