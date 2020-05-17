using System.Collections.Generic;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapExternalSecurityGroupLocatorResult
    {
        public LdapExternalSecurityGroupLocatorResult()
        {
        }

        public LdapExternalSecurityGroupLocatorResult(IList<string> groupsIds)
        {
            WasAbleToRetrieveGroups = true;
            GroupsIds = groupsIds;
        }

        public bool WasAbleToRetrieveGroups { get; set; }
        public IList<string> GroupsIds { get; set; }
    }
}