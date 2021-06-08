using Novell.Directory.Ldap;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public interface IUserPrincipalFinder
    {
        UserPrincipal FindByIdentity(LdapContext context, string externalIdentity);

        IEnumerable<UserPrincipal> SearchUser(LdapContext context, string searchToken);
    }

    public class UserPrincipalFinder : IUserPrincipalFinder
    {
        public UserPrincipal FindByIdentity(LdapContext context, string externalIdentity)
        {
            var escapedName = ToLdapUserName(externalIdentity);
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
                    context.UserNameAttribute
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
                DN = searchResult.Dn,
                DisplayName = searchResult.TryGetAttribute(context.UserDisplayNameAttribute)?.StringValue,
                UPN = searchResult.TryGetAttribute(context.UserPrincipalNameAttribute)?.StringValue,
                Groups = searchResult.TryGetAttribute(context.UserMembershipAttribute)?.StringValueArray,
                ExternalIdentity = searchResult.TryGetAttribute(context.UserNameAttribute)?.StringValue,
                Mail = searchResult.TryGetAttribute(context.UserEmailAttribute)?.StringValue
            };
        }

        /// <summary>
        /// Escapes username for ldap queries.
        /// </summary>
        private string ToLdapUserName(string userName)
        {
            // \e is simple backslash \ in LDAP, convert to recognized delimiter.
            return userName.Replace("\\", "\\5C");
        }

        public IEnumerable<UserPrincipal> SearchUser(LdapContext context, string searchToken)
        {
            var searchTerm = $"*{ToLdapUserName(searchToken)}*";
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
                    context.UserNameAttribute
                },
                false
                );
            return lsc.Select(x => CreatePrincipalFromLdapEntry(x, context)).ToList();
        }
    }
}