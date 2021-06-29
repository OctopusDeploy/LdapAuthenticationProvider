using Novell.Directory.Ldap;
using System;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapContext : IDisposable
    {
        public LdapConnection LdapConnection { get; internal set; }

        public string UserBaseDN { get; internal set; }

        public string UserFilter { get; internal set; }

        public string GroupBaseDN { get; internal set; }

        public string GroupFilter { get; internal set; }

        public string NestedGroupFilter { get; internal set; }

        public string UniqueAccountNameAttribute { get; internal set; }

        public string UserDisplayNameAttribute { get; internal set; }

        public string UserPrincipalNameAttribute { get; internal set; }

        public string UserMembershipAttribute { get; internal set; }

        public string UserEmailAttribute { get; internal set; }

        public string GroupNameAttribute { get; internal set; }

        public void Dispose()
        {
            if(LdapConnection?.Tls == true)
                LdapConnection.StopTls();
            
            LdapConnection?.Dispose();
        }
    }
}