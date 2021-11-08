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

                Assert.Equal(4, searchResult.Groups.Length);
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup (with brackets),ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup\\# with a hash,ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup\\, with a comma,ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup * ( ) . & - _ [ ] ` ~ | @ $ % ^ ? : { } ! ',ou=Groups,dc=mycompany,dc=local", StringComparison.InvariantCultureIgnoreCase));
            }

            [Fact]
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
            internal void FindsGroupsFromOpenLDAPWithSpecialCharacters()
            {
                var partialName = "Special";
                
                ICanSearchExternalGroups fixture = FixtureHelper.CreateLdapExternalSecurityGroupLocator(ConfigurationHelper.GetOpenLdapConfiguration(), _testLogger);

                var result = fixture.Search(partialName, new CancellationToken());

                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var searchResult = ((ResultFromExtension<ExternalSecurityGroupResult>)result).Value;

                Assert.Equal(4, searchResult.Groups.Length);
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup (with brackets),ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup# with a hash,ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup\\2C with a comma,ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
                Assert.Contains(searchResult.Groups, x => x.Id.Equals("cn=SpecialGroup * ( ) . & - _ [ ] ` ~ | @ $ % ^ ? : { } ! ',ou=Groups,dc=domain1,dc=local", StringComparison.InvariantCultureIgnoreCase));
            }
        }
    }
}