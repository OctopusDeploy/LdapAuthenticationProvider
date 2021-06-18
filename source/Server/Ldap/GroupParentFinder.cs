using System.Collections.Generic;
using System.Linq;
using Novell.Directory.Ldap;
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
            var attributesToRetrieve = new[] {"cn", "dn", "uniqueMember"};

            var result = context.LdapConnection.Search(
                context.BaseDN,
                LdapConnection.ScopeSub,
                "(&(objectClass=groupOfUniqueNames)(uniqueMember=*))".Replace("*", name.ToString()),
                attributesToRetrieve,
                false
            );

            return result.Select(r => r.Dn).ToGroupDistinguishedNames();
        }
    }
}