namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    static class LdapFilter
    {
        public static string EscapeForLdapSearchFilter(this string value)
        {
            // The library we are using already escapes DN characters but some special characters for Ldap search filters must be escaped,
            // Including the backslash character in the escaped DNs
            //
            // See: http://social.technet.microsoft.com/wiki/contents/articles/5392.active-directory-ldap-syntax-filters.aspx#Special_Characters
            return value
                .Replace("\\", "\\5C")
                .Replace("*", "\\2A")
                .Replace("(", "\\28")
                .Replace(")", "\\29")
                .Replace("\000", "\\00");
        }
    }
}
