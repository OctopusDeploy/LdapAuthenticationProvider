using System.Collections.Generic;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserPrincipalComparer : IEqualityComparer<UserPrincipal>
    {
        public bool Equals(UserPrincipal x, UserPrincipal y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(UserPrincipal obj)
        {
            return obj.ExternalIdentity.GetHashCode();
        }
    }
}