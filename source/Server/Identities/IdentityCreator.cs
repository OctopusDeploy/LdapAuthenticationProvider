using Octopus.Data.Model.User;
using Octopus.Server.Extensibility.Authentication.Resources.Identities;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Identities
{
    public class IdentityCreator : IIdentityCreator
    {
        public const string UserPrincipleNameClaimType = "upn";
        public const string UniqueAccountNameClaimType = "uan";

        public Identity Create(string email, string userPrincipleName, string uniqueAccountName, string displayName)
        {
            return new Identity(LdapAuthentication.ProviderName)
                .WithClaim(ClaimDescriptor.EmailClaimType, email, true)
                .WithClaim(UserPrincipleNameClaimType, userPrincipleName, true)
                .WithClaim(UniqueAccountNameClaimType, uniqueAccountName, true)
                .WithClaim(ClaimDescriptor.DisplayNameClaimType, displayName, false);
        }
    }
}