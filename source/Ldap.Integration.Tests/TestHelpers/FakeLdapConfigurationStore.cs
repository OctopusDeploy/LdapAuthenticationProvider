using System;
using Octopus.Data.Model;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal class FakeLdapConfigurationStore : ILdapConfigurationStore
    {
        private LdapConfiguration _configuration;
        public FakeLdapConfigurationStore(LdapConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool GetAllowAutoUserCreation() => _configuration.AllowAutoUserCreation;

        public string GetUserBaseDn() => _configuration.UserBaseDn;

        public string GetGroupBaseDn() => _configuration.GroupBaseDn;

        public object GetConfiguration() => _configuration;

        public SensitiveString GetConnectPassword() => _configuration.ConnectPassword;

        public string GetConnectUsername() => _configuration.ConnectUsername;

        public int GetConstraintTimeLimit() => _configuration.ConstraintTimeLimit;

        public string GetDefaultDomain() => _configuration.DefaultDomain;

        public string GetGroupFilter() => _configuration.GroupFilter;

        public string GetGroupNameAttribute() => _configuration.AttributeMapping.GroupNameAttribute;

        public bool GetIgnoreSslErrors() => _configuration.IgnoreSslErrors;

        public bool GetIsEnabled() => _configuration.IsEnabled;

        public string GetNestedGroupFilter() => _configuration.NestedGroupFilter;

        public int GetNestedGroupSearchDepth() => _configuration.NestedGroupSearchDepth;

        public int GetPort() => _configuration.Port;

        public bool GetReferralFollowingEnabled() => _configuration.ReferralFollowingEnabled;

        public int GetReferralHopLimit() => _configuration.ReferralHopLimit;

        public SecurityProtocol GetSecurityProtocol() => _configuration.SecurityProtocol;

        public string GetServer() => _configuration.Server;

        public string GetUniqueAccountNameAttribute() => _configuration.AttributeMapping.UniqueAccountNameAttribute;

        public string GetUserDisplayNameAttribute() => _configuration.AttributeMapping.UserDisplayNameAttribute;

        public string GetUserEmailAttribute() => _configuration.AttributeMapping.UserEmailAttribute;

        public string GetUserFilter() => _configuration.UserFilter;

        public string GetUserMembershipAttribute() => _configuration.AttributeMapping.UserMembershipAttribute;

        public string GetUserPrincipalNameAttribute() => _configuration.AttributeMapping.UserPrincipalNameAttribute;

        public void SetAllowAutoUserCreation(bool allowAutoUserCreation)
        {
            throw new NotImplementedException();
        }

        public void SetUserBaseDn(string userBaseDn)
        {
            throw new NotImplementedException();
        }

        public void SetGroupBaseDn(string groupBaseDn)
        {
            throw new NotImplementedException();
        }

        public void SetConfiguration(object config)
        {
            throw new NotImplementedException();
        }

        public void SetConnectPassword(SensitiveString password)
        {
            throw new NotImplementedException();
        }

        public void SetConnectUsername(string username)
        {
            throw new NotImplementedException();
        }

        public void SetConstraintTimeLimit(int timeLimit)
        {
            throw new NotImplementedException();
        }

        public void SetDefaultDomain(string defaultDomain)
        {
            throw new NotImplementedException();
        }

        public void SetGroupFilter(string groupFilter)
        {
            throw new NotImplementedException();
        }

        public void SetGroupNameAttribute(string groupNameAttribute)
        {
            throw new NotImplementedException();
        }

        public void SetIgnoreSslErrors(bool ignoreSslErrors)
        {
            throw new NotImplementedException();
        }

        public void SetIsEnabled(bool isEnabled)
        {
            throw new NotImplementedException();
        }

        public void SetNestedGroupFilter(string nestedGroupFilter)
        {
            throw new NotImplementedException();
        }

        public void SetNestedGroupSearchDepth(int nestedGroupSearchDepth)
        {
            throw new NotImplementedException();
        }

        public void SetPort(int port)
        {
            throw new NotImplementedException();
        }

        public void SetReferralFollowingEnabled(bool enabled)
        {
            throw new NotImplementedException();
        }

        public void SetReferralHopLimit(int hopLimit)
        {
            throw new NotImplementedException();
        }

        public void SetSecurityProtocol(SecurityProtocol securityProtocol)
        {
            throw new NotImplementedException();
        }

        public void SetServer(string server)
        {
            throw new NotImplementedException();
        }

        public void SetUniqueAccountNameAttribute(string uniqueAccountNameAttribute)
        {
            throw new NotImplementedException();
        }

        public void SetUserDisplayNameAttribute(string userDisplayNameAttribute)
        {
            throw new NotImplementedException();
        }

        public void SetUserEmailAttribute(string userEmailAttribute)
        {
            throw new NotImplementedException();
        }

        public void SetUserFilter(string userFilter)
        {
            throw new NotImplementedException();
        }

        public void SetUserMembershipAttribute(string userMembershipAttribute)
        {
            throw new NotImplementedException();
        }

        public void SetUserPrincipalNameAttribute(string userPrincipalNameAttribute)
        {
            throw new NotImplementedException();
        }
    }
}
