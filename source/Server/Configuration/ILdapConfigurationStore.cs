using Octopus.Data.Model;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Configuration
{
    public interface ILdapConfigurationStore : IExtensionConfigurationStore<LdapConfiguration>
    {
        string GetServer();
        void SetServer(string server);

        int GetPort();
        void SetPort(int port);

        void SetEncryptionMethod(EncryptionMethod encryptionMethod);
        EncryptionMethod GetEncryptionMethod();

        void SetIgnoreSslErrors(bool ignoreSslErrors);
        bool GetIgnoreSslErrors();

        string GetConnectUsername();
        void SetConnectUsername(string username);

        SensitiveString GetConnectPassword();
        void SetConnectPassword(SensitiveString password);

        string GetBaseDn();
        void SetBaseDn(string baseDn);

        string GetDefaultDomain();
        void SetDefaultDomain(string defaultDomain);

        string GetUniqueAccountNameAttribute();
        void SetUniqueAccountNameAttribute(string uniqueAccountNameAttribute);

        string GetUserFilter();
        void SetUserFilter(string userFilter);

        string GetGroupFilter();
        void SetGroupFilter(string groupFilter);

        string GetNestedGroupFilter();
        void SetNestedGroupFilter(string nestedGroupFilter);

        int GetNestedGroupSearchDepth();
        void SetNestedGroupSearchDepth(int nestedGroupSearchDepth);

        bool GetAllowAutoUserCreation();
        void SetAllowAutoUserCreation(bool allowAutoUserCreation);

        string GetUserDisplayNameAttribute();
        void SetUserDisplayNameAttribute(string userDisplayNameAttribute);

        string GetUserPrincipalNameAttribute();
        void SetUserPrincipalNameAttribute(string userPrincipalNameAttribute);

        string GetUserMembershipAttribute();
        void SetUserMembershipAttribute(string userMembershipAttribute);

        string GetUserEmailAttribute();
        void SetUserEmailAttribute(string userEmailAttribute);

        string GetGroupNameAttribute();
        void SetGroupNameAttribute(string groupNameAttribute);
        
        bool GetReferralFollowingEnabled();
        void SetReferralFollowingEnabled(bool enabled);

        int GetReferralHopLimit();
        void SetReferralHopLimit(int hopLimit);

        int GetConstraintTimeLimit();
        void SetConstraintTimeLimit(int timeLimit);
    }
}