using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public static class LdapEntryExtensions
    {
        public static LdapAttribute TryGetAttribute(this LdapEntry entry, string attributeName)
        {
            try
            {
                return entry.GetAttribute(attributeName);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}