using System.Collections.Generic;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserPrincipal
    {
        public string SamAccountName { get; set; }

        public string DisplayName { get; set; }

        public string DN { get; set; }

        public string UPN { get; set; }

        public string Mail { get; set; }

        public IEnumerable<string> Groups { get; set; }
    }
}