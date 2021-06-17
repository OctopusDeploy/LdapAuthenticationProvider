using System;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    /// <summary>
    /// Exception type to distinguish between exceptions from the Novell LDAP library and this extension
    /// </summary>
    public class LdapAuthenticationException : Exception
    {
        public LdapAuthenticationException()
        {
        }

        public LdapAuthenticationException(string message) : base(message)
        {
        }

        public LdapAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}