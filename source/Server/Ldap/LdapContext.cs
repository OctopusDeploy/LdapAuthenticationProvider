using Novell.Directory.Ldap;
using System;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapContext : IDisposable
    {
        public LdapContext(LdapConnection ldapConnection, string userBaseDN, string userFilter, string groupBaseDN, string groupFilter, string? nestedGroupFilter, string? uniqueAccountNameAttribute, string? userDisplayNameAttribute, string? userPrincipalNameAttribute, string? userMembershipAttribute, string? userEmailAttribute, string? groupNameAttribute)
        {
            LdapConnection = ldapConnection;
            UserBaseDN = userBaseDN;
            UserFilter = userFilter;
            GroupBaseDN = groupBaseDN;
            GroupFilter = groupFilter;
            NestedGroupFilter = nestedGroupFilter;
            UniqueAccountNameAttribute = uniqueAccountNameAttribute;
            UserDisplayNameAttribute = userDisplayNameAttribute;
            UserPrincipalNameAttribute = userPrincipalNameAttribute;
            UserMembershipAttribute = userMembershipAttribute;
            UserEmailAttribute = userEmailAttribute;
            GroupNameAttribute = groupNameAttribute;
        }

        public LdapConnection LdapConnection { get; private set; }

        public string? UserBaseDN { get; private set; }

        public string? UserFilter { get; private set; }

        public string? GroupBaseDN { get; private set; }

        public string? GroupFilter { get; private set; }

        public string? NestedGroupFilter { get; private set; }

        public string? UniqueAccountNameAttribute { get; private set; }

        public string? UserDisplayNameAttribute { get; private set; }

        public string? UserPrincipalNameAttribute { get; private set; }

        public string? UserMembershipAttribute { get; private set; }

        public string? UserEmailAttribute { get; private set; }

        public string? GroupNameAttribute { get; private set; }

        public void Dispose()
        {
            if(LdapConnection?.Tls == true)
                LdapConnection.StopTls();
            
            LdapConnection?.Dispose();
        }
    }
}