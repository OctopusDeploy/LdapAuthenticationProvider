using System;
using NSubstitute;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Ldap;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;
using Xunit.Abstractions;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal static class FixtureHelper
    {
        public static UserMatcher CreateUserMatcher(LdapConfiguration configuration, ITestOutputHelper testLogger)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);
            var log = new TestLogger(testLogger);

            return new UserMatcher(
                new LdapContextProvider(new Lazy<ILdapConfigurationStore>(() => configurationStore), log),
                new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore)),
                configurationStore,
                new UserPrincipalFinder(),
                new IdentityCreator());
        }

        public static LdapUserCreationFromPrincipal CreateLdapUserCreationFromPrincipal(LdapConfiguration configuration, string userStoreUserName, ITestOutputHelper testLogger)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);
            var nameNormalizer = new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore));

            var log = new TestLogger(testLogger);
            var ldapService = new LdapService(
                log,
                nameNormalizer,
                new LdapContextProvider(new Lazy<ILdapConfigurationStore>(() => configurationStore), log),
                new UserPrincipalFinder());

            var credentialValidator = new LdapCredentialValidator(
                log,
                nameNormalizer,
                new FakeUpdateableUserStore(),
                configurationStore,
                new IdentityCreator(),
                ldapService);

            return new LdapUserCreationFromPrincipal(configurationStore, credentialValidator);
        }

        public static LdapCredentialValidator CreateLdapCredentialValidator(LdapConfiguration configuration, string userStoreUserName, ITestOutputHelper testLogger)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);
            var nameNormalizer = new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore));

            var log = new TestLogger(testLogger);
            var ldapService = new LdapService(
                log,
                nameNormalizer,
                new LdapContextProvider(new Lazy<ILdapConfigurationStore>(() => configurationStore), log),
                new UserPrincipalFinder());

            return new LdapCredentialValidator(
                log,
                nameNormalizer,
                new FakeUpdateableUserStore(),
                configurationStore,
                new IdentityCreator(),
                ldapService);
        }

        public static GroupRetriever CreateFixtureGroupRetriever(LdapConfiguration configuration, ITestOutputHelper testLogger)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);
            var log = new TestLogger(testLogger);

            var groupLocator = new LdapExternalSecurityGroupLocator(
                log,
                new LdapContextProvider(new Lazy<ILdapConfigurationStore>(() => configurationStore), log),
                new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore)),
                configurationStore,
                new UserPrincipalFinder(),
                new NestedGroupFinder(new GroupParentFinder()));

            return new GroupRetriever(log, configurationStore, groupLocator);
        }

        public static LdapExternalSecurityGroupLocator CreateLdapExternalSecurityGroupLocator(LdapConfiguration configuration, ITestOutputHelper testLogger)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);
            var log = new TestLogger(testLogger);

            return new LdapExternalSecurityGroupLocator(
                log,
                new LdapContextProvider(new Lazy<ILdapConfigurationStore>(() => configurationStore), log),
                new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore)),
                new FakeLdapConfigurationStore(configuration),
                new UserPrincipalFinder(),
                new NestedGroupFinder(new GroupParentFinder())
                );
        }

        public static UserSearch CreateUserSearch(LdapConfiguration configuration, ITestOutputHelper testLogger)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);
            var log = new TestLogger(testLogger);

            return new UserSearch(
                new LdapContextProvider(new Lazy<ILdapConfigurationStore>(() => configurationStore), log),
                new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore)),
                new FakeLdapConfigurationStore(configuration),
                new UserPrincipalFinder(),
                new IdentityCreator()
                );
        }
    }
}
