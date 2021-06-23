using System.Collections.Generic;
using System.Linq;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface INestedGroupFinder
    {
        IEnumerable<GroupDistinguishedName> FindAllParentGroups(LdapContext context, int depth, IEnumerable<GroupDistinguishedName> names);
    }

    public class NestedGroupFinder : INestedGroupFinder
    {
        readonly IGroupParentFinder parentFinder;

        public NestedGroupFinder(IGroupParentFinder parentFinder)
        {
            this.parentFinder = parentFinder;
        }

        /// <returns>Returns all ancestor DNs, as well as the passed in group DNs.</returns>
        public IEnumerable<GroupDistinguishedName> FindAllParentGroups(LdapContext context, int searchDepth, IEnumerable<GroupDistinguishedName> names)
        {
            var groups = new HashSet<GroupDistinguishedName>(names.Distinct());
            var nextGroups = groups;

            while (nextGroups.Any() && searchDepth > 0)
            {
                var newGroups = nextGroups.SelectMany(group => parentFinder.FindParentGroups(context, group));

                //If we already have a returned group in the groups list, then we don't want to add it again. This also stops circular references from causing issues.
                nextGroups = newGroups.Where(ng => !groups.Contains(ng)).ToHashSet();
                groups.UnionWith(nextGroups);

                searchDepth--;
            }

            return groups;
        }
    }
}