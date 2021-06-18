using System.Collections.Generic;
using System.Linq;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface INestedGroupFinder
    {
        IEnumerable<GroupDistinguishedName> GetAllParentGroups(LdapContext context, int depth, IEnumerable<GroupDistinguishedName> names);
    }

    public class NestedGroupFinder : INestedGroupFinder
    {
        readonly IGroupParentFinder parentFinder;

        public NestedGroupFinder(IGroupParentFinder parentFinder)
        {
            this.parentFinder = parentFinder;
        }

        public IEnumerable<GroupDistinguishedName> GetAllParentGroups(LdapContext context, int depth, IEnumerable<GroupDistinguishedName> names)
        {
            var groups = new HashSet<GroupDistinguishedName>(names.Distinct());
            var nextGroups = (IEnumerable<GroupDistinguishedName>)groups;

            while (nextGroups.Any() && depth > 0)
            {
                var newGroups = nextGroups.SelectMany(group => parentFinder.FindParentGroups(context, group));

                //If we already have a returned group in the groups list, then we don't want to add it again.
                nextGroups = newGroups.Where(ng => !groups.Contains(ng));
                groups.UnionWith(nextGroups);

                depth--;
            }

            return groups;
        }
    }
}