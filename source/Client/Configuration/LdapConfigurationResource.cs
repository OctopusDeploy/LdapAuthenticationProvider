using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Model;
using Octopus.Client.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Client.Extensibility.Authentication.Ldap.Configuration
{
   public class LdapConfigurationResource : ExtensionConfigurationResource
    {
        public LdapConfigurationResource()
        {
            Id = "authentication-ldap";
        }

        public const string ServerDescription = "Set the server URL.";
        public const string PortDescription = "Set the port using to connect.";
        public const string UseSslDescription = "Sets whether to use Secure Socket Layer to connect to LDAP.";
        public const string IgnoreSslErrorsDescription = "Sets whether to ignore certificate validation errors.";
        public const string UsernameDescription = "Set the user DN to query LDAP.";
        public const string PasswordDescription = "Set the password to query LDAP.";
        public const string UserBaseDnDescription = "Set the root distinguished name (DN) to query LDAP for Users.";
        public const string DefaultDomainDescription = "Set the default domain when none is given in the logon form. Optional.";
        public const string UserFilterDescription = "The filter to use when searching valid users.";
        public const string GroupBaseDnDescription = "Set the root distinguished name (DN) to query LDAP for Groups.";
        public const string GroupFilterDescription = "The filter to use when searching valid user groups.";
        public const string AllowAutoUserCreationDescription = "Whether unknown users will be automatically created upon successful login.";

        [DisplayName("Server")]
        [Description(ServerDescription)]
        [Writeable]
        public string Server { get; set; }

        [DisplayName("Port")]
        [Description(PortDescription)]
        [Writeable]
        public int Port { get; set; }

        [DisplayName("Use SSL")]
        [Description(UseSslDescription)]
        [Writeable]
        public bool UseSsl { get; set; }

        [DisplayName("Ignore SSL errors")]
        [Description(IgnoreSslErrorsDescription)]
        [Writeable]
        public bool IgnoreSslErrors { get; set; }

        [DisplayName("Username")]
        [Description(UsernameDescription)]
        [Writeable]
        public string ConnectUsername { get; set; }

        [DisplayName("Password")]
        [Description(PasswordDescription)]
        [Writeable]
        public SensitiveValue ConnectPassword { get; set; }

        [DisplayName("User Base DN")]
        [Description(UserBaseDnDescription)]
        [Writeable]
        public string UserBaseDN { get; set; }

        [DisplayName("Default Domain")]
        [Description(DefaultDomainDescription)]
        [Writeable]
        public string DefaultDomain { get; set; }

        [DisplayName("User Filter")]
        [Description(UserFilterDescription)]
        [Writeable]
        public string UserFilter { get; set; }

        [DisplayName("Group Base DN")]
        [Description(GroupBaseDnDescription)]
        [Writeable]
        public string GroupBaseDN { get; set; }

        [DisplayName("Group Filter")]
        [Description(GroupFilterDescription)]
        [Writeable]
        public string GroupFilter { get; set; }

        [DisplayName("Allow Auto User Creation")]
        [Description(AllowAutoUserCreationDescription)]
        [Writeable]
        public bool AllowAutoUserCreation { get; set; }

        [DisplayName("Attribute Mapping")]
        public LdapMappingConfigurationResource AttributeMapping { get; set; } = new LdapMappingConfigurationResource();
    }

    public class LdapMappingConfigurationResource
    {
        public const string UserNameAttributeDescription = "Set the name of the LDAP attribute containing the username, which is used to authenticate via the logon form.";
        public const string UserDisplayNameAttributeDescription = "Set the name of the LDAP attribute containing the user's full name.";
        public const string UserPrincipalNameAttributeDescription = "Set the name of the LDAP attribute containing the user's principal name.";
        public const string UserMembershipAttributeDescription = "Set the name of the LDAP attribute to use when loading the user's groups.";
        public const string UserEmailAttributeDescription = "Set the name of the LDAP attribute containing the user's email address.";
        public const string GroupNameAttributeDescription = "Set the name of the LDAP attribute containing the group's name.";

        [DisplayName("Username Attribute")]
        [Description(UserNameAttributeDescription)]
        [Writeable]
        public string UserNameAttribute { get; set; }

        [DisplayName("User Display Name Attribute")]
        [Description(UserDisplayNameAttributeDescription)]
        [Writeable]
        public string UserDisplayNameAttribute { get; set; }

        [DisplayName("User Principal Name Attribute")]
        [Description(UserPrincipalNameAttributeDescription)]
        [Writeable]
        public string UserPrincipalNameAttribute { get; set; }

        [DisplayName("User Membership Attribute")]
        [Description(UserMembershipAttributeDescription)]
        [Writeable]
        public string UserMembershipAttribute { get; set; }

        [DisplayName("User Email Attribute")]
        [Description(UserEmailAttributeDescription)]
        [Writeable]
        public string UserEmailAttribute { get; set; }

        [DisplayName("Group Name Attribute")]
        [Description(GroupNameAttributeDescription)]
        [Writeable]
        public string GroupNameAttribute { get; set; }
    }
}