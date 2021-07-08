using Octopus.Data.Model;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.HostServices.Mapping;
using System.Collections.Generic;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Configuration
{
    public class LdapConfigurationSettings :
        ExtensionConfigurationSettings<LdapConfiguration, LdapConfigurationResource,
            ILdapConfigurationStore>, ILdapConfigurationSettings
    {
        public LdapConfigurationSettings(
            ILdapConfigurationStore configurationDocumentStore) : base(configurationDocumentStore)
        {
        }

        public override string Id => LdapConfigurationStore.SingletonId;

        public override string ConfigurationSetName => "LDAP";

        public override string Description => "LDAP authentication settings";

        public override IEnumerable<IConfigurationValue> GetConfigurationValues()
        {
            var isEnabled = ConfigurationDocumentStore.GetIsEnabled();

            yield return new ConfigurationValue<bool>("Octopus.WebPortal.LdapIsEnabled", isEnabled, isEnabled, "Is Enabled");
            yield return new ConfigurationValue<string?>("Octopus.WebPortal.LdapServer", ConfigurationDocumentStore.GetServer(), isEnabled && !string.IsNullOrWhiteSpace(ConfigurationDocumentStore.GetServer()), "Server");
            yield return new ConfigurationValue<int>("Octopus.WebPortal.LdapPort", ConfigurationDocumentStore.GetPort(), isEnabled, "Port");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapSecurityProtocol", ConfigurationDocumentStore.GetSecurityProtocol().ToString(), isEnabled, "Security Protocol");
            yield return new ConfigurationValue<bool>("Octopus.WebPortal.LdapIgnoreSslErrors", ConfigurationDocumentStore.GetIgnoreSslErrors(), isEnabled, "Ignore SSL errors");
            yield return new ConfigurationValue<string?>("Octopus.WebPortal.LdapUsername", ConfigurationDocumentStore.GetConnectUsername(), isEnabled, "Username");
            yield return new ConfigurationValue<SensitiveString?>("Octopus.WebPortal.LdapPassword", ConfigurationDocumentStore.GetConnectPassword(), isEnabled, "Password");
            yield return new ConfigurationValue<string?>("Octopus.WebPortal.LdapUserBaseDn", ConfigurationDocumentStore.GetUserBaseDn(), isEnabled, "User Base DN");
            yield return new ConfigurationValue<string?>("Octopus.WebPortal.LdapDefaultDomain", ConfigurationDocumentStore.GetDefaultDomain(), isEnabled, "Default Domain");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUserFilter", ConfigurationDocumentStore.GetUserFilter(), isEnabled, "User Filter");
            yield return new ConfigurationValue<string?>("Octopus.WebPortal.LdapGroupBaseDn", ConfigurationDocumentStore.GetGroupBaseDn(), isEnabled, "Group Base DN");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapGroupFilter", ConfigurationDocumentStore.GetGroupFilter(), isEnabled, "Group Filter");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapNestedGroupFilter", ConfigurationDocumentStore.GetNestedGroupFilter(), isEnabled, "Nested Group Filter");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapNestedGroupSearchDepth", ConfigurationDocumentStore.GetNestedGroupSearchDepth().ToString(), isEnabled, "Nested Group Search Depth");
            yield return new ConfigurationValue<bool>("Octopus.WebPortal.LdapAllowAutoUserCreation=", ConfigurationDocumentStore.GetAllowAutoUserCreation(), isEnabled, "Allow Auto User Creation");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapReferralFollowingEnabled", ConfigurationDocumentStore.GetReferralFollowingEnabled().ToString(), isEnabled, "Referral Following Enabled");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapConstraintTimeLimit", ConfigurationDocumentStore.GetConstraintTimeLimit().ToString(), isEnabled, "Constraint Time Limit (seconds)");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapReferralHopLimit", ConfigurationDocumentStore.GetReferralHopLimit().ToString(), isEnabled, "Referral Hop Limit");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUniqueAccountNameAttribute", ConfigurationDocumentStore.GetUniqueAccountNameAttribute(), isEnabled, "Unique Account Name Attribute");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUserDisplayNameAttribute", ConfigurationDocumentStore.GetUserDisplayNameAttribute(), isEnabled, "User Display Name Attribute");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUserPrincipalNameAttribute", ConfigurationDocumentStore.GetUserPrincipalNameAttribute(), isEnabled, "User Principal Name Attribute");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUserMembershipAttribute", ConfigurationDocumentStore.GetUserMembershipAttribute(), isEnabled, "User Membership Attribute");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUserEmailAttribute", ConfigurationDocumentStore.GetUserEmailAttribute(), isEnabled, "User Email Attribute");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapGroupNameAttribute", ConfigurationDocumentStore.GetGroupNameAttribute(), isEnabled, "Group Name Attribute");
        }

        public override void BuildMappings(IResourceMappingsBuilder builder)
        {
            builder.Map<LdapConfigurationResource, LdapConfiguration>();
            builder.Map<LdapMappingConfigurationResource, LdapMappingConfiguration>();
        }
    }
}