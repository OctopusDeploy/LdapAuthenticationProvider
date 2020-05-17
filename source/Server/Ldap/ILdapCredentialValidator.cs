using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Storage.User;
using System.Threading;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface ILdapCredentialValidator : IDoesBasicAuthentication
    {
        AuthenticationUserCreateResult GetOrCreateUser(string username, CancellationToken cancellationToken);
    }
}