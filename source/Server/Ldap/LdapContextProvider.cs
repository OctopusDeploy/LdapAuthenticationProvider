using Novell.Directory.Ldap;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using System;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapContextProvider : ILdapContextProvider
    {
        private readonly Lazy<ILdapConfigurationStore> ldapConfiguration;

        public LdapContextProvider(Lazy<ILdapConfigurationStore> ldapConfiguration)
        {
            this.ldapConfiguration = ldapConfiguration;
        }

        public LdapContext GetContext()
        {
            var con = new LdapConnection();
            con.Connect(ldapConfiguration.Value.GetServer(), ldapConfiguration.Value.GetPort());
            con.Bind(ldapConfiguration.Value.GetConnectUsername(), ldapConfiguration.Value.GetConnectPassword().Value);

            return new LdapContext
            {
                LdapConnection = con,
                BaseDN = ldapConfiguration.Value.GetBaseDn(),
                UserNameAttribute = ldapConfiguration.Value.GetUserNameAttribute(),
                UserFilter = ldapConfiguration.Value.GetUserFilter(),
                GroupFilter = ldapConfiguration.Value.GetGroupFilter(),
                GroupNameAttribute = ldapConfiguration.Value.GetGroupNameAttribute(),
                UserDisplayNameAttribute = ldapConfiguration.Value.GetUserDisplayNameAttribute(),
                UserEmailAttribute = ldapConfiguration.Value.GetUserEmailAttribute(),
                UserMembershipAttribute = ldapConfiguration.Value.GetUserMembershipAttribute(),
                UserPrincipalNameAttribute = ldapConfiguration.Value.GetUserPrincipalNameAttribute()
            };
        }
    }
}