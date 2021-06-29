using System;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal static class ConfigurationHelper
    {
        const string ENVVAR_AD_SERVER = "OCTOPUS_LDAP_AD_SERVER";
        const string ENVVAR_AD_PORT = "OCTOPUS_LDAP_AD_PORT";
        const string ENVVAR_AD_USER = "OCTOPUS_LDAP_AD_USER";
        const string ENVVAR_AD_PASSWORD = "OCTOPUS_LDAP_AD_PASSWORD";

        const string ENVVAR_OPENLDAP_SERVER = "OCTOPUS_LDAP_OPENLDAP_SERVER";
        const string ENVVAR_OPENLDAP_PORT = "OCTOPUS_LDAP_OPENLDAP_PORT";
        const string ENVVAR_OPENLDAP_USER = "OCTOPUS_LDAP_OPENLDAP_USER";
        const string ENVVAR_OPENLDAP_PASSWORD = "OCTOPUS_LDAP_OPENLDAP_PASSWORD";

        public static LdapConfiguration GetActiveDirectoryConfiguration()
        {
            var server = Environment.GetEnvironmentVariable(ENVVAR_AD_SERVER);
            var port = Convert.ToInt32(Environment.GetEnvironmentVariable(ENVVAR_AD_PORT) ?? "389");
            var user = Environment.GetEnvironmentVariable(ENVVAR_AD_USER);
            var password = Environment.GetEnvironmentVariable(ENVVAR_AD_PASSWORD);

            return new LdapConfiguration()
                .Enabled()
                .WithConnection(
                    server: server,
                    port: port,
                    user: user,
                    password: password)
                .WithUserSettings(
                    userBaseDn: "cn=Users,dc=mycompany,dc=local",
                    userFilter: "(&(objectClass=person)(sAMAccountName=*))")
                .WithUserAttributes(
                    uniqueAccountNameAttribute: "sAMAccountName",
                    userDisplayNameAttribute: "displayName",
                    userPrincipalNameAttribute: "userPrincipalName",
                    userMembershipAttribute: "memberOf",
                    userEmailAttribute: "mail")
                .WithGroupSettings(
                    groupBaseDn: "ou=Groups,dc=mycompany,dc=local",
                    groupFilter: "(&(objectClass=group)(cn=*))",
                    nestedGroupFilter: "(&(objectClass=group)(member=*))")
                .WithGroupAttributes(
                    groupNameAttribute: "cn");
        }

        public static LdapConfiguration GetOpenLdapConfiguration()
        {
            var server = Environment.GetEnvironmentVariable(ENVVAR_OPENLDAP_SERVER);
            var port = Convert.ToInt32(Environment.GetEnvironmentVariable(ENVVAR_OPENLDAP_PORT));
            var user = Environment.GetEnvironmentVariable(ENVVAR_OPENLDAP_USER);
            var password = Environment.GetEnvironmentVariable(ENVVAR_OPENLDAP_PASSWORD);

            return new LdapConfiguration()
                .Enabled()
                .WithConnection(
                    server: server,
                    port: port,
                    user: user,
                    password: password)
                .WithUserSettings(
                    userBaseDn: "dc=domain1,dc=local",
                    userFilter: "(&(objectClass=inetOrgPerson)(cn=*))")
                .WithUserAttributes(
                    uniqueAccountNameAttribute: "cn",
                    userDisplayNameAttribute: "displayname",
                    userPrincipalNameAttribute: "cn",
                    userMembershipAttribute: "memberOf",
                    userEmailAttribute: "mail")
                .WithGroupSettings(
                    groupBaseDn: "dc=domain1,dc=local",
                    groupFilter: "(&(objectClass=groupOfUniqueNames)(cn=*))",
                    nestedGroupFilter: "(&(objectClass=groupOfUniqueNames)(uniqueMember=*))")
                .WithGroupAttributes(
                    groupNameAttribute: "cn");
        }
    }
}
