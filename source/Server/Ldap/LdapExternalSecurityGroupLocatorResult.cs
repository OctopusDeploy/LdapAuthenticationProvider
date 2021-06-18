using System.Collections.Generic;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapExternalSecurityGroupLocatorResult
    {
        public bool WasAbleToRetrieveGroups { get; }
        public IEnumerable<GroupDistinguishedName> GroupDistinguishedNames { get; }

        public LdapExternalSecurityGroupLocatorResult()
        {
            WasAbleToRetrieveGroups = false;
            GroupDistinguishedNames = new List<GroupDistinguishedName>();
        }

        public LdapExternalSecurityGroupLocatorResult(IEnumerable<GroupDistinguishedName> groupDistinguishedNames)
        {
            WasAbleToRetrieveGroups = true;
            GroupDistinguishedNames = groupDistinguishedNames;
        }
    }
}