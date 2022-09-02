using System;
using System.Threading;
using Ldap.Integration.Tests.TestHelpers;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Results;
using Xunit;
using Xunit.Abstractions;

namespace Ldap.Integration.Tests
{
    public class ICanSearchExternalGroupsTests
    {
        public class TheSearchMethod
        {
            readonly ITestOutputHelper _testLogger;
            public TheSearchMethod(ITestOutputHelper testLogger)
            {
                _testLogger = testLogger;
            }

            [Fact]
            [Trait("AuthProvider","ActiveDirectory")]
            internal void FindsGroupsFromActiveDirectory()
            {
                var partialName = "Devel";

                ICanSearchExternalGroups fixture = FixtureHelper.CreateLdapExternalSecurityGroupLocator(ConfigurationHelper.GetActiveDirectoryConfiguration(), _testLogger);

                var result = fixture.Search(partialName, new CancellationToken());
                
                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var searchResult = ((ResultFromExtension<ExternalSecurityGroupResult>)result).Value;

                Assert.Equal(3, searchResult.Groups.Length);
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=Developers,ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=DeveloperGroup1,ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=DeveloperGroup2,ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
            }

            [Fact]
            internal void FindsGroupsFromActiveDirectoryWithSpecialCharacters()
            {
                var partialName = "Special";

                ICanSearchExternalGroups fixture = FixtureHelper.CreateLdapExternalSecurityGroupLocator(ConfigurationHelper.GetActiveDirectoryConfiguration(), _testLogger);

                var result = fixture.Search(partialName, new CancellationToken());
                
                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var searchResult = ((ResultFromExtension<ExternalSecurityGroupResult>)result).Value;

                // Note that ActiveDirectory returns the reserved DN characters in escaped backslashformat, eg \, for , (comma)
                Assert.Equal(3, searchResult.Groups.Length);
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup Parent,ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup \\, \\\\ \\# \\+ \\< \\> \\; \\\" \\=,ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup * ( ) . & - _ [ ] ` ~ | @ $ % ^ ? : { } ! ',ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
            }

            [Fact]
            [Trait("AuthProvider","OpenLDAP")]
            internal void FindsGroupsFromOpenLDAP()
            {
                var partialName = "Devel";
                
                ICanSearchExternalGroups fixture = FixtureHelper.CreateLdapExternalSecurityGroupLocator(ConfigurationHelper.GetOpenLdapConfiguration(), _testLogger);

                var result = fixture.Search(partialName, new CancellationToken());

                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var searchResult = ((ResultFromExtension<ExternalSecurityGroupResult>)result).Value;

                Assert.Equal(3, searchResult.Groups.Length);
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=Developers,ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=DeveloperGroup1,ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=DeveloperGroup2,ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
            }

            [Fact]
            [Trait("AuthProvider","OpenLDAP")]
            internal void FindsGroupsFromOpenLDAPWithSpecialCharacters()
            {
                var partialName = "Special";
                
                ICanSearchExternalGroups fixture = FixtureHelper.CreateLdapExternalSecurityGroupLocator(ConfigurationHelper.GetOpenLdapConfiguration(), _testLogger);

                var result = fixture.Search(partialName, new CancellationToken());

                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var searchResult = ((ResultFromExtension<ExternalSecurityGroupResult>)result).Value;

                // Note that openLdap returns the reserved DN characters in escaped hex format, eg \2C for , (comma) and strangely doesnt encode the pound (#) symbol
                Assert.Equal(3, searchResult.Groups.Length);
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup Parent,ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup \\2C \\5C # \\2B \\3C \\3E \\3B \\22 \\3D,ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup * ( ) . & - _ [ ] ` ~ | @ $ % ^ ? : { } ! ',ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}