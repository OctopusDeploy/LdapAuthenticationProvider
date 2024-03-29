﻿using System.Collections.Generic;
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
        public IEnumerable<GroupDistinguishedName> FindParentGroups(LdapContext context, GroupDistinguishedName groupDistinguishedName)
        {
            var result = context.SearchParentGroups(groupDistinguishedName);

            return result.Select(r => r.Dn).ToGroupDistinguishedNames();
        }
    }
}