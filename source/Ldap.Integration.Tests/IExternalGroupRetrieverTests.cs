using System;
using System.Linq;
using Ldap.Integration.Tests.TestHelpers;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Results;
using Xunit;
using Xunit.Abstractions;

namespace Ldap.Integration.Tests
{
    public class IExternalGroupRetrieverTests
    {
        public class TheReadMethod
        {
            readonly ITestOutputHelper _testLogger;
            public TheReadMethod(ITestOutputHelper testLogger)
            {
                _testLogger = testLogger;
            }
            
            [Fact]
            internal void ReadsGroupsForAUserFromActiveDirectory()
            {
                // Arrange
                var user = new FakeUser("developer1");

                var expectedGroups = new[]
                {
                    "cn=DeveloperGroup1,ou=Groups,dc=mycompany,dc=local",
                    "cn=Developers,ou=Groups,dc=mycompany,dc=local",
                    "cn=Maintainers,ou=Groups,dc=mycompany,dc=local",
                };

                IExternalGroupRetriever fixture = FixtureHelper.CreateFixtureGroupRetriever(ConfigurationHelper.GetActiveDirectoryConfiguration(), _testLogger);

                // Act
                var result = fixture.Read(user, new System.Threading.CancellationToken());

                // Assert
                if (result is FailureResultFromExtension)
                {
                    throw new Exception(string.Join("\n", (result as FailureResultFromExtension).Errors));
                }

                var groupResult = (ResultFromExtension<ExternalGroupResult>)result;

                Assert.Equal(expectedGroups.Select(x => x.ToLowerInvariant()).OrderBy(x => x), groupResult.Value.GroupIds.Select(x => x.ToLowerInvariant()).OrderBy(x => x));
            }

            [Fact]
            internal void ReadsGroupsForAUserFromActiveDirectoryWithSpecialCharacters()
            {
                // Arrange
                var user = new FakeUser("special#1");

                var expectedGroups = new[]
                {
                    "cn=SpecialGroup Parent,ou=Groups,dc=mycompany,dc=local",
                    "cn=SpecialGroup \\, \\\\ \\# \\+ \\< \\> \\; \\\" \\=,ou=Groups,dc=mycompany,dc=local",
                    "cn=SpecialGroup * ( ) . & - _ [ ] ` ~ | @ $ % ^ ? : { } ! ',ou=Groups,dc=mycompany,dc=local"
                };

                IExternalGroupRetriever fixture = FixtureHelper.CreateFixtureGroupRetriever(ConfigurationHelper.GetActiveDirectoryConfiguration(), _testLogger);

                // Act
                var result = fixture.Read(user, new System.Threading.CancellationToken());

                // Assert
                if (result is FailureResultFromExtension)
                {
                    throw new Exception(string.Join("\n", (result as FailureResultFromExtension).Errors));
                }

                var groupResult = (ResultFromExtension<ExternalGroupResult>)result;

                Assert.Equal(expectedGroups.Select(x => x.ToLowerInvariant()).OrderBy(x => x), groupResult.Value.GroupIds.Select(x => x.ToLowerInvariant()).OrderBy(x => x));
            }

            [Fact]
            internal void ReadsGroupsForAUserFromOpenLDAP()
            {
                // Arrange
                var user = new FakeUser("developer1");

                // Note that ActiveDirectory returns the reserved DN characters in escaped backslashformat, eg \, for , (comma)
                var expectedGroups = new[]
                {
                    "cn=Maintainers,ou=Groups,dc=domain1,dc=local",
                    "cn=Developers,ou=Groups,dc=domain1,dc=local",
                    "cn=DeveloperGroup1,ou=Groups,dc=domain1,dc=local"
                };

                IExternalGroupRetriever fixture = FixtureHelper.CreateFixtureGroupRetriever(ConfigurationHelper.GetOpenLdapConfiguration(), _testLogger);

                // Act
                var result = fixture.Read(user, new System.Threading.CancellationToken());

                // Assert
                if (result is FailureResultFromExtension)
                {
                    throw new Exception(string.Join("\n", (result as FailureResultFromExtension).Errors));
                }

                var groupResult = (ResultFromExtension<ExternalGroupResult>)result;

                Assert.Equal(expectedGroups.Select(x => x.ToLowerInvariant()).OrderBy(x => x), groupResult.Value.GroupIds.Select(x => x.ToLowerInvariant()).OrderBy(x => x));
            }

            [Fact]
            internal void ReadsGroupsForAUserFromOpenLDAPWithSpecialCharacters()
            {
                // Arrange
                var user = new FakeUser("special#1");
                
                // Note that openLdap returns the reserved DN characters in escaped hex format, eg \2C for , (comma) and strangely doesnt encode the pound (#) symbol
                var expectedGroups = new[]
                {
                    "cn=SpecialGroup Parent,ou=Groups,dc=domain1,dc=local",
                    "cn=SpecialGroup \\2C \\5C # \\2B \\3C \\3E \\3B \\22 \\3D,ou=Groups,dc=domain1,dc=local",
                    "cn=SpecialGroup * ( ) . & - _ [ ] ` ~ | @ $ % ^ ? : { } ! ',ou=Groups,dc=domain1,dc=local"
                };

                IExternalGroupRetriever fixture = FixtureHelper.CreateFixtureGroupRetriever(ConfigurationHelper.GetOpenLdapConfiguration(), _testLogger);

                // Act
                var result = fixture.Read(user, new System.Threading.CancellationToken());

                // Assert
                if (result is FailureResultFromExtension)
                {
                    throw new Exception(string.Join("\n", (result as FailureResultFromExtension).Errors));
                }

                var groupResult = (ResultFromExtension<ExternalGroupResult>)result;

                Assert.Equal(expectedGroups.Select(x => x.ToLowerInvariant()).OrderBy(x => x), groupResult.Value.GroupIds.Select(x => x.ToLowerInvariant()).OrderBy(x => x));
            }
        }
    }
}