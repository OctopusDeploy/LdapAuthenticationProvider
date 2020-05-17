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
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapServer", ConfigurationDocumentStore.GetServer(), isEnabled && !string.IsNullOrWhiteSpace(ConfigurationDocumentStore.GetServer()), "Server");
            yield return new ConfigurationValue<int>("Octopus.WebPortal.LdapPort", ConfigurationDocumentStore.GetPort(), isEnabled, "Port");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUsername", ConfigurationDocumentStore.GetConnectUsername(), isEnabled, "Username");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapPassword", ConfigurationDocumentStore.GetConnectPassword(), isEnabled, "Password", true);
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapBaseDn", ConfigurationDocumentStore.GetBaseDn(), isEnabled, "Base DN");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapDefaultDomain", ConfigurationDocumentStore.GetBaseDn(), isEnabled, "Default Domain");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUserFilter", ConfigurationDocumentStore.GetUserFilter(), isEnabled, "User Filter");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapGroupFilter", ConfigurationDocumentStore.GetGroupFilter(), isEnabled, "Group Filter");
            yield return new ConfigurationValue<string>("Octopus.WebPortal.LdapUserNameAttribute", ConfigurationDocumentStore.GetUserNameAttribute(), isEnabled, "User Name Attribute");
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