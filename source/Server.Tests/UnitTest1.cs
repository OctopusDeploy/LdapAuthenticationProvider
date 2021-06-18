using System;
using System.Collections.Generic;
using System.Threading;
using NSubstitute;
using Octopus.Data.Model;
using Octopus.Data.Model.User;
using Octopus.Data.Storage.Configuration;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Ldap;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Xunit;

namespace Server.Tests
{
    public class GroupTests
    {
        [Fact]
        public void Test1()
        {
            var log = Substitute.For<ISystemLog>();
            var store = Substitute.For<ILdapConfigurationStore>();

            //for name normalizer
            store.GetDefaultDomain().Returns("");

            //for context provider
            store.GetUseSsl().Returns(false);
            store.GetServer().Returns("localhost");
            store.GetPort().Returns(389);
            store.GetConnectUsername().Returns("cn=admin,dc=domain1,dc=local");
            store.GetConnectPassword().Returns("test1234".ToSensitiveString());
            store.GetReferralFollowingEnabled().Returns(false);
            store.GetReferralHopLimit().Returns(0);
            store.GetConstraintTimeLimit().Returns(5);
            store.GetBaseDn().Returns("dc=domain1,dc=local");
            store.GetUniqueAccountNameAttribute().Returns("cn");
            store.GetUserFilter().Returns("(&(objectClass=inetOrgPerson)(cn=*))");
            store.GetGroupFilter().Returns("(&(objectClass=groupOfUniqueNames)(cn=*))");
            store.GetGroupNameAttribute().Returns("cn");
            store.GetUserDisplayNameAttribute().Returns("displayName");
            store.GetUserEmailAttribute().Returns("mail");
            store.GetUserMembershipAttribute().Returns("memberOf");
            store.GetUserPrincipalNameAttribute().Returns("cn");

            //for group retriever
            store.GetIsEnabled().Returns(true);

            var lazyStore = new Lazy<ILdapConfigurationStore>(store);
            var contextProvider = new LdapContextProvider(lazyStore, log);
            var principalFinder = new UserPrincipalFinder();

            var locator = new LdapExternalSecurityGroupLocator(log, contextProvider, new LdapObjectNameNormalizer(lazyStore), store, principalFinder);

           // var groupIds = locator.GetGroupIdsForUser("maintainer", new CancellationToken());

            var retriever = new GroupRetriever(log, store, locator);
            var user = Substitute.For<IUser>();
            user.Identities.Returns(new HashSet<Identity>());

            var identity = new Identity("LDAP");

            identity.Claims.Add("email", new IdentityClaim("webdeveloper@octopus.com", true, false));
            identity.Claims.Add("upn", new IdentityClaim("webdeveloper", true, false));
            identity.Claims.Add("uan", new IdentityClaim("webdeveloper", true, false));
            identity.Claims.Add("dn", new IdentityClaim("Web Developer User", false, false));
            
            user.Identities.Add(identity);
            var groups = retriever.Read(user, new CancellationToken(false));
        }
    }
}



/*var principalFinder = Substitute.For<IUserPrincipalFinder>();

principalFinder.FindByIdentity(Arg.Any<LdapContext>(), Arg.Any<string>()).Returns(_ =>
{
    return new UserPrincipal()
    {
        DistinguishedName = "cn=maintainer,dc=domain1,dc=local",
        DisplayName = "Maintainer User",
        UserPrincipalName = "Maintainer",
        Groups = new []{"Maintaners"},
        UniqueAccountName = "maintainer",
        Email = "maintainer@gmail.com"
    };
});*/