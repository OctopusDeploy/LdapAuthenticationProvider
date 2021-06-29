using System;
using Octopus.Data.Model;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal static class LdapConfigurationExtensions
    {
        internal static LdapConfiguration Enabled(this LdapConfiguration configuration)
        {
            configuration.IsEnabled = true;

            return configuration;
        }

        internal static LdapConfiguration WithConnection(this LdapConfiguration configuration, string server, int port, string user, string password)
        {
            configuration.Server = server ?? throw new ArgumentNullException(nameof(server));
            configuration.Port = port;
            configuration.ConnectUsername = user ?? throw new ArgumentNullException(nameof(user));
            configuration.ConnectPassword = password.ToSensitiveString() ?? throw new ArgumentNullException(nameof(password));

            return configuration;
        }

        internal static LdapConfiguration WithUserSettings(this LdapConfiguration configuration, string baseDn, string userFilter)
        {
            configuration.BaseDn = baseDn;
            configuration.UserFilter = userFilter;

            return configuration;
        }

        internal static LdapConfiguration WithUserAttributes(this LdapConfiguration configuration, string uniqueAccountNameAttribute, string userDisplayNameAttribute, string userPrincipalNameAttribute, string userMembershipAttribute, string userEmailAttribute)
        {
            configuration.AttributeMapping.UniqueAccountNameAttribute = uniqueAccountNameAttribute;
            configuration.AttributeMapping.UserDisplayNameAttribute = userDisplayNameAttribute;
            configuration.AttributeMapping.UserPrincipalNameAttribute = userPrincipalNameAttribute;
            configuration.AttributeMapping.UserMembershipAttribute = userMembershipAttribute;
            configuration.AttributeMapping.UserEmailAttribute = userEmailAttribute;

            return configuration;
        }

        internal static LdapConfiguration WithGroupSettings(this LdapConfiguration configuration, string groupBaseDn, string groupFilter, string nestedGroupFilter)
        {
            configuration.GroupBaseDn = groupBaseDn;
            configuration.GroupFilter = groupFilter;
            configuration.NestedGroupFilter = nestedGroupFilter;

            return configuration;
        }

        internal static LdapConfiguration WithGroupAttributes(this LdapConfiguration configuration, string groupNameAttribute)
        {
            configuration.AttributeMapping.GroupNameAttribute = groupNameAttribute;

            return configuration;
        }
    }
}
