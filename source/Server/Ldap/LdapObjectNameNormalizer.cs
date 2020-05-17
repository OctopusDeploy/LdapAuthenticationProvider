using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using System;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapObjectNameNormalizer : ILdapObjectNameNormalizer
    {
        private readonly Lazy<ILdapConfigurationStore> ldapConfiguration;

        public LdapObjectNameNormalizer(Lazy<ILdapConfigurationStore> ldapConfiguration)
        {
            this.ldapConfiguration = ldapConfiguration;
        }

        public string BuildUserName(string name, string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
                return name;
            return $"{domain}\\{name}";
        }

        public void NormalizeName(string name, out string namePart, out string domainPart)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!TryParseDownLevelLogonName(name, out namePart, out domainPart))
            {
                namePart = name;
                domainPart = !string.IsNullOrWhiteSpace(ldapConfiguration.Value.GetDefaultDomain())
                    ? ldapConfiguration.Value.GetDefaultDomain()
                    : null;
            }
        }

        // If the return value is true, dlln was a valid down-level logon name, and name/domain
        // contain precisely the component name and domain name values. Note, we don't split
        // UPNs this way because the suffix part of a UPN is not necessarily a domain, and in
        // the default LogonUser case should be passed whole to the function with a null domain.
        private static bool TryParseDownLevelLogonName(string dlln, out string username, out string domain)
        {
            if (dlln == null) throw new ArgumentNullException(nameof(dlln));
            username = null;
            domain = null;

            var slash = dlln.IndexOf('\\');
            if (slash == -1 || slash == dlln.Length - 1 || slash == 0)
                return false;

            domain = dlln.Substring(0, slash).Trim();
            username = dlln.Substring(slash + 1).Trim();
            return !string.IsNullOrWhiteSpace(domain) && !string.IsNullOrWhiteSpace(username);
        }
    }
}