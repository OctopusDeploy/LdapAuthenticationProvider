using System.Threading;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface ILdapService
    {
        UserValidationResult ValidateCredentials(string username, string password, CancellationToken cancellationToken);

        UserValidationResult FindByIdentity(string username);
    }
}