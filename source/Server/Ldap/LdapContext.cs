using Novell.Directory.Ldap;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapContext : IDisposable
    {
        readonly LdapConnection ldapConnection;
        readonly ISystemLog log;

        public LdapContext(LdapConnection ldapConnection, ISystemLog log)
        {
            this.ldapConnection = ldapConnection;
            this.log = log;
        }

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

        internal ILdapSearchResults SearchUsers(string searchToken)
        {
            var escapedValue = $"*{searchToken.EscapeForLdapSearchFilter(log)}*";
            var searchFilter = UserFilter?.Replace("*", escapedValue);

            var results = SearchLdap(
                UserBaseDN,
                searchFilter,
                new[]
                {
                    "cn",
                    UserDisplayNameAttribute,
                    UserMembershipAttribute,
                    UserPrincipalNameAttribute,
                    UserEmailAttribute,
                    UniqueAccountNameAttribute
                }
            );

            return results;
        }

        internal LdapEntry FindUser(string userName)
        {
            var escapedValue = userName.EscapeForLdapSearchFilter(log);
            var searchFilter = UserFilter?.Replace("*", escapedValue);

            var results = SearchLdap(
                UserBaseDN,
                searchFilter,
                new[]
                {
                    "cn",
                    UserDisplayNameAttribute,
                    UserMembershipAttribute,
                    UserPrincipalNameAttribute,
                    UserEmailAttribute,
                    UniqueAccountNameAttribute
                }
            );

            return results?.FirstOrDefault();
        }

        internal List<LdapEntry> SearchGroups(string searchToken)
        {
            var escapedValue = $"*{searchToken.EscapeForLdapSearchFilter(log)}*";
            var searchFilter = GroupFilter.Replace("*", escapedValue);

            var results = SearchLdap(
                GroupBaseDN,
                searchFilter,
                new[]
                {
                    "dn",
                    GroupNameAttribute
                }
            );

            return results.ToList();
        }

        internal List<LdapEntry> SearchParentGroups(GroupDistinguishedName groupDistinguishedName)
        {
            var escapedValue = groupDistinguishedName.ToString().EscapeForLdapSearchFilter(log);
            var searchFilter = NestedGroupFilter.Replace("*", escapedValue);

            var results = SearchLdap(
                GroupBaseDN,
                searchFilter,
                new[]
                {
                    "dn",
                    GroupNameAttribute
                }
            );

            return results.ToList();
        }

        private ILdapSearchResults SearchLdap(string searchBase, string escapedearchFilter, string[] attributes)
        {
            log.Verbose($"LDAP::SearchLdap (Base: '{searchBase}' Filter: '{escapedearchFilter}' Attributes: '{string.Join(",", attributes)}')");
            return ldapConnection.Search(
                searchBase,
                LdapConnection.ScopeSub,
                escapedearchFilter,
                attributes,
                false);
        }

        public void ValidateCredentials(string distinguishedName, string password)
        {
            log.Verbose($"Calling LDAP::Bind ('{distinguishedName}')");
            ldapConnection.Bind(distinguishedName, password);
        }

        public void Dispose()
        {
            if(ldapConnection?.Tls == true)
                ldapConnection.StopTls();
            
            ldapConnection?.Dispose();
        }
    }
}