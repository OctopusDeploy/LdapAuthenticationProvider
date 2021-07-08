using System;
using System.Collections.Generic;
using System.Linq;
using Novell.Directory.Ldap;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface IGroupParentFinder
    {
        IEnumerable<GroupDistinguishedName> FindParentGroups(LdapContext context, GroupDistinguishedName name);
    }

    public class GroupParentFinder : IGroupParentFinder
    {
        public IEnumerable<GroupDistinguishedName> FindParentGroups(LdapContext context, GroupDistinguishedName name)
        {
            if (string.IsNullOrEmpty(context.GroupBaseDN))
                throw new ArgumentNullException(nameof(context.GroupBaseDN));

            if (string.IsNullOrEmpty(context.GroupFilter))
                throw new ArgumentNullException(nameof(context.GroupFilter));

            if (string.IsNullOrEmpty(context.NestedGroupFilter))
                throw new ArgumentNullException(nameof(context.NestedGroupFilter));

            var attributesToRetrieve = new[] {"cn", "dn" };

            var result = context.LdapConnection.Search(
                context.GroupBaseDN,
                LdapConnection.ScopeSub,
                context.NestedGroupFilter.Replace("*", name.ToString()),
                attributesToRetrieve,
                false
            );

            return result.Select(r => r.Dn).ToGroupDistinguishedNames();
        }
    }
}