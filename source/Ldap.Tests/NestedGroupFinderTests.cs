using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Octopus.Server.Extensibility.Authentication.Ldap;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;
using Xunit;

namespace Ldap.Tests
{
    public class NestedGroupFinderTests
    {
        [Fact]
        public void Test1()
        {
            var parentFinder = new ParentFinder(GetGroups());
            var context = Substitute.For<LdapContext>();

            var nestedFinder = new NestedGroupFinder(parentFinder);

            var allGroups = nestedFinder.FindAllParentGroups(context, 5,
                new [] {
                    "122".ToGroupDistinguishedName(),
                    "12".ToGroupDistinguishedName()
                });
        }

        public class ParentFinder : IGroupParentFinder
        {
            readonly Group[] groups;

            public ParentFinder(Group[] groups)
            {
                this.groups = groups;
            }

            public IEnumerable<GroupDistinguishedName> FindParentGroups(LdapContext context, GroupDistinguishedName name)
            {
                //Get all the groups who have a child named name
                return groups.Where(g => g.Children.Any(c => c.Name == name))
                    .Select(g => g.Name);
            }
        }

        public Group[] GetGroups()
        {
            var root1 = new Group("1");
            var child11 = new Group("11", root1);
            var child12 = new Group("12", root1);
            var child111 = new Group("111", child11);
            var child112 = new Group("112", child11);

            return new []{root1, child11, child12, child111, child112};
        }

        public class Group
        {
            internal GroupDistinguishedName Name { get; }
            internal List<Group> Children { get; } = new List<Group>();
            internal Group Parent { get; }

            internal Group(string name)
            {
                Name = name.ToGroupDistinguishedName();
            }
            internal Group(string name, Group parent) : this(name)
            {
                parent.Children.Add(this);
                Parent = parent;
            }
        }
    }
}