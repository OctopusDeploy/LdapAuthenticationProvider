using Octopus.Data.Model.User;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Results;
using System.Threading;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface ILdapCredentialValidator : IDoesBasicAuthentication
    {
        IResultFromExtension<IUser> GetOrCreateUser(string username, CancellationToken cancellationToken);
    }
}