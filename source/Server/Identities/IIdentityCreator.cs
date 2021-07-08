using Octopus.Data.Model.User;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Identities
{
    public interface IIdentityCreator
    {
        Identity Create(string? email, string? userPrincipleName, string? uniqueAccountName, string? displayName);
    }
}