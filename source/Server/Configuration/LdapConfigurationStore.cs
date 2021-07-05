using System;
using Octopus.Data.Model;
using Octopus.Data.Storage.Configuration;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Configuration
{
    public class LdapConfigurationStore : ExtensionConfigurationStore<LdapConfiguration>, ILdapConfigurationStore
    {
        public static string SingletonId = "authentication-ldap";
        ISystemLog log;

        public LdapConfigurationStore(IConfigurationStore configurationStore, ISystemLog log) : base(configurationStore)
        {
            this.log = log;
        }

        public override string Id => SingletonId;

        public string GetServer() => GetProperty(doc => doc.Server);
        public void SetServer(string server) => SetProperty(doc => doc.Server = server);

        public int GetPort() => GetProperty(doc => doc.Port);
        public void SetPort(int port) => SetProperty(doc => doc.Port = port);

        public void SetSecurityProtocol(SecurityProtocol securityProtocol) => SetProperty(doc => doc.SecurityProtocol = securityProtocol);
        public SecurityProtocol GetSecurityProtocol() => GetProperty(doc => doc.SecurityProtocol);

        public void SetIgnoreSslErrors(bool ignoreSslErrors) => SetProperty(doc => doc.IgnoreSslErrors = ignoreSslErrors);
        public bool GetIgnoreSslErrors() => GetProperty(doc => doc.IgnoreSslErrors);

        public string GetConnectUsername() => GetProperty(doc => doc.ConnectUsername);
        public void SetConnectUsername(string username) => SetProperty(doc => doc.ConnectUsername = username);

        public SensitiveString GetConnectPassword() => GetProperty(doc => doc.ConnectPassword);
        public void SetConnectPassword(SensitiveString password) => SetProperty(doc =>
        {
            log.WithSensitiveValue(password.Value);
            doc.ConnectPassword = password;
        });

        public string GetUserBaseDn() => GetProperty(doc => doc.UserBaseDn);
        public void SetUserBaseDn(string userBaseDn) => SetProperty(doc => doc.UserBaseDn = userBaseDn);

        public string GetGroupBaseDn() => GetProperty(doc => doc.GroupBaseDn);
        public void SetGroupBaseDn(string groupBaseDn) => SetProperty(doc => doc.GroupBaseDn = groupBaseDn);

        public string GetDefaultDomain() => GetProperty(doc => doc.DefaultDomain);
        public void SetDefaultDomain(string defaultDomain) => SetProperty(doc => doc.DefaultDomain = defaultDomain);

        public string GetUniqueAccountNameAttribute() => GetProperty(doc => doc.AttributeMapping.UniqueAccountNameAttribute);
        public void SetUniqueAccountNameAttribute(string uniqueAccountNameAttribute) => SetProperty(doc => doc.AttributeMapping.UniqueAccountNameAttribute = uniqueAccountNameAttribute);

        public string GetUserFilter() => GetProperty(doc => doc.UserFilter);
        public void SetUserFilter(string userFilter) => SetProperty(doc => doc.UserFilter = userFilter);

        public string GetGroupFilter() => GetProperty(doc => doc.GroupFilter);
        public void SetGroupFilter(string groupFilter) => SetProperty(doc => doc.GroupFilter = groupFilter);
        
        public string GetNestedGroupFilter() => GetProperty(doc => doc.NestedGroupFilter);
        public void SetNestedGroupFilter(string nestedGroupFilter) => SetProperty(doc => doc.NestedGroupFilter = nestedGroupFilter);

        public int GetNestedGroupSearchDepth() => GetProperty(doc => doc.NestedGroupSearchDepth);
        public void SetNestedGroupSearchDepth(int nestedGroupSearchDepth) => SetProperty(doc => doc.NestedGroupSearchDepth = nestedGroupSearchDepth);

        public bool GetAllowAutoUserCreation() => GetProperty(doc => doc.AllowAutoUserCreation);
        public void SetAllowAutoUserCreation(bool allowAutoUserCreation) => SetProperty(doc => doc.AllowAutoUserCreation = allowAutoUserCreation);

        //Attributes
        public string GetUserDisplayNameAttribute() => GetProperty(doc => doc.AttributeMapping.UserDisplayNameAttribute);
        public void SetUserDisplayNameAttribute(string userDisplayNameAttribute) => SetProperty(doc => doc.AttributeMapping.UserDisplayNameAttribute = userDisplayNameAttribute);

        public string GetUserPrincipalNameAttribute() => GetProperty(doc => doc.AttributeMapping.UserPrincipalNameAttribute);
        public void SetUserPrincipalNameAttribute(string userPrincipalNameAttribute) => SetProperty(doc => doc.AttributeMapping.UserPrincipalNameAttribute = userPrincipalNameAttribute);

        public string GetUserMembershipAttribute() => GetProperty(doc => doc.AttributeMapping.UserMembershipAttribute);
        public void SetUserMembershipAttribute(string userMembershipAttribute) => SetProperty(doc => doc.AttributeMapping.UserMembershipAttribute = userMembershipAttribute);

        public string GetUserEmailAttribute() => GetProperty(doc => doc.AttributeMapping.UserEmailAttribute);
        public void SetUserEmailAttribute(string userEmailAttribute) => SetProperty(doc => doc.AttributeMapping.UserEmailAttribute = userEmailAttribute);

        public string GetGroupNameAttribute() => GetProperty(doc => doc.AttributeMapping.GroupNameAttribute);
        public void SetGroupNameAttribute(string groupNameAttribute) => SetProperty(doc => doc.AttributeMapping.GroupNameAttribute = groupNameAttribute);
        
        public bool GetReferralFollowingEnabled() => GetProperty(doc => doc.ReferralFollowingEnabled);
        public void SetReferralFollowingEnabled(bool enabled) => SetProperty(doc => doc.ReferralFollowingEnabled = enabled);

        public int GetReferralHopLimit() => GetProperty(doc => doc.ReferralHopLimit);
        public void SetReferralHopLimit(int hopLimit) => SetProperty(doc => doc.ReferralHopLimit = hopLimit);

        public int GetConstraintTimeLimit() => GetProperty(doc => doc.ConstraintTimeLimit);
        public void SetConstraintTimeLimit(int timeLimit) => SetProperty(doc => doc.ConstraintTimeLimit = timeLimit);
    }
}