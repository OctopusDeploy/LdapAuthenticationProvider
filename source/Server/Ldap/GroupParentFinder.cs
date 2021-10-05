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
            var attributesToRetrieve = new[] {"cn", "dn" };

            var result = context.LdapConnection.Search(
                context.GroupBaseDN,
                LdapConnection.ScopeSub,
                context.NestedGroupFilter.Replace("*", name.ToString().EscapeForLdapSearchFilter()),
                attributesToRetrieve,
                false
            );

            return result.Select(r => r.Dn).ToGroupDistinguishedNames();
        }
    }
}