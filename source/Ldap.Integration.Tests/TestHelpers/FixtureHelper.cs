using System;
using NSubstitute;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Ldap;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal static class FixtureHelper
    {
        public static UserMatcher CreateUserMatcher(LdapConfiguration configuration)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);

            return new UserMatcher(
                new LdapContextProvider(new Lazy<ILdapConfigurationStore>(() => configurationStore), Substitute.For<ISystemLog>()),
                new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore)),
                configurationStore,
                new UserPrincipalFinder(),
                new IdentityCreator());
        }

        public static LdapUserCreationFromPrincipal CreateLdapUserCreationFromPrincipal(LdapConfiguration configuration, string userStoreUserName)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);
            var nameNormalizer = new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore));

            var log = Substitute.For<ISystemLog>();
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

        public static GroupRetriever CreateFixtureGroupRetriever(LdapConfiguration configuration)
        {
            var configurationStore = new FakeLdapConfigurationStore(configuration);
            var log = Substitute.For<ISystemLog>();

            var groupLocator = new LdapExternalSecurityGroupLocator(
                log,
                new LdapContextProvider(new Lazy<ILdapConfigurationStore>(() => configurationStore), log),
                new LdapObjectNameNormalizer(new Lazy<ILdapConfigurationStore>(() => configurationStore)),
                configurationStore,
                new UserPrincipalFinder(),
                new NestedGroupFinder(new GroupParentFinder()));

            return new GroupRetriever(log, configurationStore, groupLocator);
        }
    }
}
