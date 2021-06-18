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
            var escapedName = ToLdapUniqueAccountName(uniqueAccountName);
            var lsc = context.LdapConnection.Search(
                context.BaseDN,
                LdapConnection.ScopeSub,
                context.UserFilter?.Replace("*", escapedName),
                new[]
                {
                    "cn",
                    context.UserDisplayNameAttribute,
                    context.UserMembershipAttribute,
                    context.UserPrincipalNameAttribute,
                    context.UserEmailAttribute,
                    context.UniqueAccountNameAttribute
                },
                false
                );
            var searchResult = lsc.FirstOrDefault();
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

        /// <summary>
        /// Escapes uniqueAccountName for ldap queries.
        /// </summary>
        private string ToLdapUniqueAccountName(string uniqueAccountName)
        {
            // \e is simple backslash \ in LDAP, convert to recognized delimiter.
            return uniqueAccountName.Replace("\\", "\\5C");
        }

        public IEnumerable<UserPrincipal> SearchUser(LdapContext context, string searchToken)
        {
            var searchTerm = $"*{ToLdapUniqueAccountName(searchToken)}*";
            var lsc = context.LdapConnection.Search(
                context.BaseDN,
                LdapConnection.ScopeSub,
                context.UserFilter?.Replace("*", searchTerm),
                new[]
                {
                    "cn",
                    context.UserDisplayNameAttribute,
                    context.UserMembershipAttribute,
                    context.UserPrincipalNameAttribute,
                    context.UserEmailAttribute,
                    context.UniqueAccountNameAttribute
                },
                false
                );
            return lsc.Select(x => CreatePrincipalFromLdapEntry(x, context)).ToList();
        }
    }
}