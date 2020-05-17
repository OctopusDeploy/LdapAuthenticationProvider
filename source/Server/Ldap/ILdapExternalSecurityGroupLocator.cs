using Octopus.Server.Extensibility.Authentication.Extensions;
using System.Threading;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface ILdapExternalSecurityGroupLocator : ICanSearchExternalGroups
    {
        LdapExternalSecurityGroupLocatorResult GetGroupIdsForUser(string samAccountName, CancellationToken cancellationToken);
    }
}