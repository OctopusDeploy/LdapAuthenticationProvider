using System.Linq;
using System.Threading;
using Ldap.Integration.Tests.TestHelpers;
using Octopus.Data.Model.User;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Results;
using Xunit;
using Xunit.Abstractions;

namespace Ldap.Integration.Tests
{
    public class IDoesBasicAuthenticationTests
    {
        public class TheValidateCredentialsMethod
        {
            readonly ITestOutputHelper _testLogger;
            public TheValidateCredentialsMethod(ITestOutputHelper testLogger)
            {
                _testLogger = testLogger;
            }

            [Fact]
            internal void ValidatesAUserFromActiveDirectory()
            {
                // Arrange
                var userName = "developer1";
                var password = "devp@ss01!";

                IDoesBasicAuthentication fixture = FixtureHelper.CreateLdapCredentialValidator(ConfigurationHelper.GetActiveDirectoryConfiguration(), userName, _testLogger);

                // Act
                var result = fixture.ValidateCredentials(userName, password, new CancellationToken());

                // Assert
                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var user = ((ResultFromExtension<IUser>)result).Value;

                Assert.Equal("developer1@mycompany.local", user.Username);
                Assert.Equal("Developer User 1", user.DisplayName);
                Assert.Equal("developer1@mycompany.local", user.EmailAddress);
                Assert.Equal("developer1", user.Identities.First().Claims["uan"].Value);
                Assert.Equal("developer1@mycompany.local", user.Identities.First().Claims["upn"].Value);
                Assert.Equal("developer1@mycompany.local", user.Identities.First().Claims["email"].Value);
                Assert.Equal("Developer User 1", user.Identities.First().Claims["dn"].Value);
            }

            [Fact]
            internal void ValidatesAUserFromActiveDirectoryWithSpecialCharacters()
            {
                // Arrange
                var userName = "special#1";
                var password = "devp@ss01!";

                IDoesBasicAuthentication fixture = FixtureHelper.CreateLdapCredentialValidator(ConfigurationHelper.GetActiveDirectoryConfiguration(), userName, _testLogger);

                // Act
                var result = fixture.ValidateCredentials(userName, password, new CancellationToken());

                // Assert
                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var user = ((ResultFromExtension<IUser>)result).Value;

                Assert.Equal("special#1@mycompany.local", user.Username);
                Assert.Equal("Special User #1", user.DisplayName);
                Assert.Equal("special#1@mycompany.local", user.EmailAddress);
                Assert.Equal("special#1", user.Identities.First().Claims["uan"].Value);
                Assert.Equal("special#1@mycompany.local", user.Identities.First().Claims["upn"].Value);
                Assert.Equal("special#1@mycompany.local", user.Identities.First().Claims["email"].Value);
                Assert.Equal("Special User #1", user.Identities.First().Claims["dn"].Value);
            }

            [Fact]
            internal void ValidatesAUserFromOpenLDAP()
            {
                // Arrange
                var userName = "developer1";
                var password = "developer_pass";

                IDoesBasicAuthentication fixture = FixtureHelper.CreateLdapCredentialValidator(ConfigurationHelper.GetOpenLdapConfiguration(), userName, _testLogger);

                // Act
                var result = fixture.ValidateCredentials(userName, password, new CancellationToken());

                // Assert
                ExtensionResultHelper.AssertSuccesfulExtensionResult(result);
                var createdUser = ((ResultFromExtension<IUser>)result).Value;

                Assert.Equal("developer1", createdUser.Username);
                Assert.Equal("Developer User 1", createdUser.DisplayName);
                Assert.Equal("developer1@gmail.com", createdUser.EmailAddress);
                Assert.Equal("developer1", createdUser.Identities.First().Claims["uan"].Value);
                Assert.Equal("developer1", createdUser.Identities.First().Claims["upn"].Value);
                Assert.Equal("developer1@gmail.com", createdUser.Identities.First().Claims["email"].Value);
                Assert.Equal("Developer User 1", createdUser.Identities.First().Claims["dn"].Value);
            }
        }
    }
}