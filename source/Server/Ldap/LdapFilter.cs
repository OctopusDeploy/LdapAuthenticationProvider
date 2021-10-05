using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    static class LdapFilter
    {
        public static string EscapeForLdapSearchFilter(this string value)
        {
            return value
                .Replace("\\", "\\5C");
        }
    }
}
