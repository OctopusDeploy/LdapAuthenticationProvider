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
        [Theory]
        [MemberData(nameof(GetTestData))]
        internal void FindAllParentGroupsTest(NestedGroupTestDataSource testData)
        {
            var parentFinder = new ParentFinder(testData.Groups);
            var context = Substitute.For<LdapContext>();
            var nestedFinder = new NestedGroupFinder(parentFinder);
            var results = nestedFinder.FindAllParentGroups(context, testData.SearchDepth, testData.InitialGroups);

            var sortedResults = results.OrderBy(r => r);
            var sortedExpected = testData.ExpectedResult.OrderBy(r => r);

            Assert.Equal(sortedResults, sortedExpected);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            yield return WhenHasHierarchy_ThenOnlyReturnBranchContainingInitialGroup().AsObjectArray();
            yield return WhenHasCircularReferences_ThenStopAtLoop().AsObjectArray();
            yield return WhenHasMoreParentsThanSearchDepth_ThenStopAtSearchDepth().AsObjectArray();
            yield return WhenHasHierarchy_ThenOnlyReturnBranchesForInitialGroups().AsObjectArray();
        }

        static NestedGroupTestDataSource WhenHasHierarchy_ThenOnlyReturnBranchContainingInitialGroup()
        {
            var root1 = new Group("1");
            var child11 = new Group("11", root1);
            var child12 = new Group("12", root1);
            var child111 = new Group("111", child11);
            var child112 = new Group("112", child11);

            return new NestedGroupTestDataSource(
                testName: nameof(WhenHasHierarchy_ThenOnlyReturnBranchContainingInitialGroup),
                groups: new[] {root1, child11, child12, child111, child112},
                initialGroups: new[] {child111},
                searchDepth: 3,
                expectedResult: new[] {root1, child11, child111}
            );
        }

        static NestedGroupTestDataSource WhenHasHierarchy_ThenOnlyReturnBranchesForInitialGroups()
        {
            var root1 = new Group("1");
            var child11 = new Group("11", root1);
            var child12 = new Group("12", root1);
            var child13 = new Group("13", root1);
            var child111 = new Group("111", child11);
            var child112 = new Group("112", child11);
            var child121 = new Group("121", child12);
            var child122 = new Group("122", child12);
            var child131 = new Group("131", child13);

            return new NestedGroupTestDataSource(
                testName: nameof(WhenHasHierarchy_ThenOnlyReturnBranchesForInitialGroups),
                groups: new[] {root1, child11, child12, child13, child111, child112, child121, child122, child131},
                initialGroups: new[] {child111, child122},
                searchDepth: 10,
                expectedResult: new[] {root1, child11, child111, child12, child122}
            );
        }

        static NestedGroupTestDataSource WhenHasCircularReferences_ThenStopAtLoop()
        {
            var root1 = new Group("1");
            var child11 = new Group("11", root1);
            var child111 = new Group("111", child11);
            child111.Children.Add(root1);

            return new NestedGroupTestDataSource(
                testName: nameof(WhenHasCircularReferences_ThenStopAtLoop),
                groups: new[] {root1, child11, child111},
                initialGroups: new[] {child111},
                searchDepth: 3,
                expectedResult: new[] {root1, child11, child111}
            );
        }

        static NestedGroupTestDataSource WhenHasMoreParentsThanSearchDepth_ThenStopAtSearchDepth()
        {
            var root1 = new Group("1");
            var child11 = new Group("11", root1);
            var child111 = new Group("111", child11);
            child111.Children.Add(root1);

            return new NestedGroupTestDataSource(
                testName: nameof(WhenHasMoreParentsThanSearchDepth_ThenStopAtSearchDepth),
                groups: new[] {root1, child11, child111},
                initialGroups: new[] {child111},
                searchDepth: 1, //Note that the result will include the initial group
                expectedResult: new[] { child11, child111}
            );
        }

        internal class NestedGroupTestDataSource
        {
            public IEnumerable<Group> Groups { get; }
            public string TestName { get; }
            public IEnumerable<Group> ExpectedResult { get; }
            public IEnumerable<Group> InitialGroups { get; }
            public int SearchDepth { get; }

            public NestedGroupTestDataSource(string testName, IEnumerable<Group> groups, IEnumerable<Group> initialGroups, int searchDepth, IEnumerable<Group> expectedResult)
            {
                TestName = testName;
                Groups = groups;
                InitialGroups = initialGroups;
                SearchDepth = searchDepth;
                ExpectedResult = expectedResult;
            }

            public object[] AsObjectArray()
            {
                return new object[] {this};
            }

            /// <summary>
            /// This tells us what test this is in the test result output.
            /// </summary>
            public override string ToString()
            {
                return TestName;
            }
        }

        class ParentFinder : IGroupParentFinder
        {
            readonly IEnumerable<Group> groups;

            public ParentFinder(IEnumerable<Group> groups)
            {
                this.groups = groups;
            }

            public IEnumerable<GroupDistinguishedName> FindParentGroups(LdapContext context, GroupDistinguishedName name)
            {
                //Get all the groups who have a child named name
                return groups.Where(g => g.Children.Any(c => c == name))
                    .Select(g => g);
            }
        }

        internal class Group : GroupDistinguishedName
        {
            //internal GroupDistinguishedName Name { get; }
            internal List<Group> Children { get; } = new List<Group>();
            //Group Parent { get; }

            internal Group(string name) : base(name)
            {
            }

            internal Group(string name, Group parent) : this(name)
            {
                parent.Children.Add(this);
                //Parent = parent;
            }
        }
    }
}