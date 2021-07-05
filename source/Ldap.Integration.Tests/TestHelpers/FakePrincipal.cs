using System;
using System.Security.Principal;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal class FakePrincipal : IPrincipal
    {
        public FakePrincipal(string userName)
        {
            Identity = new GenericIdentity(userName);
        }

        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }
}
