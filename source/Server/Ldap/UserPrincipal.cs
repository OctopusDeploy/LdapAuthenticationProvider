using System.Collections.Generic;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserPrincipal
    {
        public string ExternalIdentity { get; set; }

        public string DisplayName { get; set; }

        public string DistinguishedName { get; set; }

        public string UserPrincipalName { get; set; }

        public string Email { get; set; }

        public IEnumerable<string> Groups { get; set; }
    }
}