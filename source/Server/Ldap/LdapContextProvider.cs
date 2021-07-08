using Novell.Directory.Ldap;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapContextProvider : ILdapContextProvider
    {
        private readonly Lazy<ILdapConfigurationStore> ldapConfiguration;
        private readonly ISystemLog log;

        public LdapContextProvider(Lazy<ILdapConfigurationStore> ldapConfiguration, ISystemLog log)
        {
            this.ldapConfiguration = ldapConfiguration;
            this.log = log;
        }

        public LdapContext GetContext()
        {
            var options = new LdapConnectionOptions();

            if (ldapConfiguration.Value.GetSecurityProtocol() == SecurityProtocol.SSL)
            {
                options.UseSsl();
                options.ConfigureRemoteCertificateValidationCallback(RemoteCertificateValidation);
            }
            else if (ldapConfiguration.Value.GetSecurityProtocol() == SecurityProtocol.StartTLS)
            {
                options.ConfigureRemoteCertificateValidationCallback(RemoteCertificateValidation);
            }

            var userBaseDn = ldapConfiguration.Value.GetUserBaseDn() ?? throw new Exception();
            var groupBaseDn = ldapConfiguration.Value.GetGroupBaseDn() ?? throw new Exception();

            try
            {
                var server = ldapConfiguration.Value.GetServer() ?? throw new Exception("LdapConfiguration Server cannot be null!");
                var connectUser = ldapConfiguration.Value.GetConnectUsername() ?? throw new Exception("LdapConfiguration Server cannot be null!");
                var connectPassword = ldapConfiguration.Value.GetConnectPassword() ?? throw new Exception("LdapConfiguration Server cannot be null!");

                var con = new LdapConnection(options);
                con.Connect(server, ldapConfiguration.Value.GetPort());

                //This must occur after connecting, but before binding.
                if (ldapConfiguration.Value.GetSecurityProtocol() == SecurityProtocol.StartTLS)
                    con.StartTls();

                con.Bind(connectUser, connectPassword.Value);

                con.Constraints = new LdapConstraints(
                    ldapConfiguration.Value.GetConstraintTimeLimit() * 1000,
                    ldapConfiguration.Value.GetReferralFollowingEnabled(), 
                    null,
                    ldapConfiguration.Value.GetReferralHopLimit());

                return new LdapContext(
                    con,
                    userBaseDn,
                    groupBaseDn,
                    ldapConfiguration.Value.GetUniqueAccountNameAttribute(),
                    ldapConfiguration.Value.GetUserFilter(),
                    ldapConfiguration.Value.GetGroupFilter(),
                    ldapConfiguration.Value.GetNestedGroupFilter(),
                    ldapConfiguration.Value.GetGroupNameAttribute(),
                    ldapConfiguration.Value.GetUserDisplayNameAttribute(),
                    ldapConfiguration.Value.GetUserEmailAttribute(),
                    ldapConfiguration.Value.GetUserMembershipAttribute(),
                    ldapConfiguration.Value.GetUserPrincipalNameAttribute());
            }
            catch (LdapException ex)
            {
                throw new LdapAuthenticationException($"Unable to connect to the LDAP server.  Please see your administrator if this re-occurs.  Error code {ex.ResultCode}", ex);
            }
        }

        private bool RemoteCertificateValidation(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
                log.ErrorFormat("LDAP certificate validation failed: RemoteCertificateNotAvailable, {0}", certificate?.ToString() ?? "*unknown certificate*");
            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
                log.ErrorFormat("LDAP certificate validation failed: RemoteCertificateNameMismatch, {0}", certificate?.ToString() ?? "*unknown certificate*");
            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
            {
                log.ErrorFormat("LDAP certificate validation failed: RemoteCertificateChainErrors\n{0}",
                    string.Join('\n', (chain?.ChainStatus ?? Array.Empty<X509ChainStatus>()).Select(x => x.StatusInformation).ToList())
                    );
            }

            return ldapConfiguration.Value.GetIgnoreSslErrors();
        }
    }
}