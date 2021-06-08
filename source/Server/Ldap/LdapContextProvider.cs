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

        public LdapContextProvider(
            Lazy<ILdapConfigurationStore> ldapConfiguration,
            ISystemLog log
            )
        {
            this.ldapConfiguration = ldapConfiguration;
            this.log = log;
        }

        public LdapContext GetContext()
        {
            var options = new LdapConnectionOptions();

            if (ldapConfiguration.Value.GetUseSsl())
            {
                options.UseSsl();
                options.ConfigureRemoteCertificateValidationCallback(RemoteCertificateValidation);
            }

            try
            {
                var con = new LdapConnection(options);
                con.Connect(ldapConfiguration.Value.GetServer(), ldapConfiguration.Value.GetPort());
                con.Bind(ldapConfiguration.Value.GetConnectUsername(),
                    ldapConfiguration.Value.GetConnectPassword().Value);
                con.Constraints.ReferralFollowing = ldapConfiguration.Value.GetReferralFollowingEnabled();
                con.Constraints.HopLimit = ldapConfiguration.Value.GetReferralHopLimit();
                con.Constraints.TimeLimit = ldapConfiguration.Value.GetConstraintTimeLimit();

                return new LdapContext
                {
                    LdapConnection = con,
                    BaseDN = ldapConfiguration.Value.GetBaseDn(),
                    UserNameAttribute = ldapConfiguration.Value.GetUserNameAttribute(),
                    UserFilter = ldapConfiguration.Value.GetUserFilter(),
                    GroupFilter = ldapConfiguration.Value.GetGroupFilter(),
                    GroupNameAttribute = ldapConfiguration.Value.GetGroupNameAttribute(),
                    UserDisplayNameAttribute = ldapConfiguration.Value.GetUserDisplayNameAttribute(),
                    UserEmailAttribute = ldapConfiguration.Value.GetUserEmailAttribute(),
                    UserMembershipAttribute = ldapConfiguration.Value.GetUserMembershipAttribute(),
                    UserPrincipalNameAttribute = ldapConfiguration.Value.GetUserPrincipalNameAttribute()
                };
            }
            catch (LdapException ex)
            {
                throw new Exception($"Unable to connect to the LDAP server.  Please see your administrator if this re-occurs.  Error code {ex.ResultCode}");
            }
        }


        private bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
                log.ErrorFormat("LDAP certificate validation failed: RemoteCertificateNotAvailable, {0}", certificate.ToString());
            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
                log.ErrorFormat("LDAP certificate validation failed: RemoteCertificateNameMismatch, {0}", certificate.ToString());
            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
            {
                log.ErrorFormat("LDAP certificate validation failed: RemoteCertificateChainErrors\n{0}",
                    string.Join('\n', chain.ChainStatus.Select(x => x.StatusInformation).ToList())
                    );
            }

            return ldapConfiguration.Value.GetIgnoreSslErrors();
        }
    }
}