using Octopus.Data.Model;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Configuration
{
    public class LdapConfiguration : ExtensionConfigurationDocument
    {
        public LdapConfiguration() : base(LdapConfigurationStore.SingletonId, "LDAP", "Octopus Deploy (contributed by Thomas Unger)", "1.0")
        {
            AllowAutoUserCreation = true;
        }

        public string Server { get; set; }

        public int Port { get; set; } = 389;

        public SecurityProtocol SecurityProtocol { get; set; } = SecurityProtocol.None;

        public bool IgnoreSslErrors { get; set; }

        public string ConnectUsername { get; set; }

        public SensitiveString ConnectPassword { get; set; }

        public string BaseDn { get; set; }

        public string DefaultDomain { get; set; }

        public string UserFilter { get; set; } = "(&(objectClass=person)(sAMAccountName=*))";

        public string GroupFilter { get; set; } = "(&(objectClass=group)(cn=*))";

        public string NestedGroupFilter { get; set; } = "(&(objectClass=group)(uniqueMember=*))";
        public int NestedGroupSearchDepth { get; set; } = 5;
        public bool AllowAutoUserCreation { get; set; }

        public bool ReferralFollowingEnabled { get; set; } = false;

        /// <summary>
        /// Defaults to 10, as specified in the Novell LDAP library.
        /// If set to 0, no limit is imposed.
        /// </summary>
        public int ReferralHopLimit { get; set; } = 10;

        /// <summary>
        /// In ms.  Defaults to 0 (i.e. no limit)
        /// </summary>
        public int ConstraintTimeLimit { get; set; } = 0;

        public LdapMappingConfiguration AttributeMapping { get; set; } = new LdapMappingConfiguration();
    }

    public class LdapMappingConfiguration
    {
        public string UniqueAccountNameAttribute { get; set; } = "sAMAccountName";

        public string UserDisplayNameAttribute { get; set; } = "displayName";

        public string UserPrincipalNameAttribute { get; set; } = "userPrincipalName";

        public string UserMembershipAttribute { get; set; } = "memberOf";

        public string UserEmailAttribute { get; set; } = "mail";

        public string GroupNameAttribute { get; set; } = "cn";
    }
}