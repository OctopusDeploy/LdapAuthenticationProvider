using Novell.Directory.Ldap;
using System.Collections.Generic;
using System.Linq;
using Octopus.Server.Extensibility.Authentication.Ldap.Model;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface IUserPrincipalFinder
    {
        UserPrincipal FindByIdentity(LdapContext context, string uniqueAccountName);

        IEnumerable<UserPrincipal> SearchUser(LdapContext context, string searchToken);
    }

    public class UserPrincipalFinder : IUserPrincipalFinder
    {
        public UserPrincipal FindByIdentity(LdapContext context, string uniqueAccountName)
        {
            var searchResult = context.FindUser(uniqueAccountName);

            if (searchResult == null)
                return null;

            return CreatePrincipalFromLdapEntry(searchResult, context);
        }

        private UserPrincipal CreatePrincipalFromLdapEntry(LdapEntry searchResult, LdapContext context)
        {
            return new UserPrincipal
            {
                DistinguishedName = searchResult.Dn,
                DisplayName = searchResult.TryGetAttribute(context.UserDisplayNameAttribute)?.StringValue,
                UserPrincipalName = searchResult.TryGetAttribute(context.UserPrincipalNameAttribute)?.StringValue,
                Groups = searchResult.TryGetAttribute(context.UserMembershipAttribute)?.StringValueArray.ToGroupDistinguishedNames(),
                UniqueAccountName = searchResult.TryGetAttribute(context.UniqueAccountNameAttribute)?.StringValue,
                Email = searchResult.TryGetAttribute(context.UserEmailAttribute)?.StringValue
            };
        }

        public IEnumerable<UserPrincipal> SearchUser(LdapContext context, string searchToken)
        {
            var results = context.SearchUsers(searchToken);

            return results.Select(x => CreatePrincipalFromLdapEntry(x, context)).ToList();
        }
    }
}