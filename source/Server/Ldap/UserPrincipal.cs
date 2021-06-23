using System.Collections.Generic;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserPrincipal
    {
        public string UniqueAccountName { get; set; }

        public string DisplayName { get; set; }

        public string DistinguishedName { get; set; }

        public string UserPrincipalName { get; set; }

        public string Email { get; set; }

        public IEnumerable<GroupDistinguishedName> Groups { get; set; }
    }
}