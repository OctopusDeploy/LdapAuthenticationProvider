using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Extensions.Identities;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;
using Octopus.Server.Extensibility.Authentication.Resources;
using Octopus.Server.Extensibility.Authentication.Resources.Identities;
using Octopus.Server.MessageContracts;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapAuthenticationProvider : IAuthenticationProviderWithGroupSupport,
        IUseAuthenticationIdentities
    {
        private readonly ILdapConfigurationStore configurationStore;

        public LdapAuthenticationProvider(ILdapConfigurationStore configurationStore)
        {
            this.configurationStore = configurationStore;
        }

        public string IdentityProviderName => LdapAuthentication.ProviderName;

        public bool IsEnabled => configurationStore.GetIsEnabled();

        public bool SupportsPasswordManagement => false;

        public AuthenticationProviderElement GetAuthenticationProviderElement()
        {
            return new AuthenticationProviderElement
            {
                Name = IdentityProviderName,
                IdentityType = IdentityType.ActiveDirectory,
                FormsLoginEnabled = true
            };
        }

        public AuthenticationProviderThatSupportsGroups GetGroupLookupElement()
        {
            return new AuthenticationProviderThatSupportsGroups
            {
                Name = IdentityProviderName,
                IsRoleBased = false,
                SupportsGroupLookup = true,
                LookupUri = "~" + LdapApi.ApiExternalGroupsSearch
            };
        }

        public string[] GetAuthenticationUrls()
        {
            return new string[0];
        }

        public IdentityMetadataResource GetMetadata()
        {
            return new IdentityMetadataResource
            {
                IdentityProviderName = IdentityProviderName,
                ClaimDescriptors = new[]
                {
                    new ClaimDescriptor { Type = IdentityCreator.UserPrincipleNameClaimType, Label = "User principal name", IsIdentifyingClaim = true, Description = "UPN identifier."},
                    new ClaimDescriptor { Type = IdentityCreator.UniqueAccountNameClaimType, Label = "User identifier", IsIdentifyingClaim = true, Description = "User's external identifier."},
                    new ClaimDescriptor { Type = ClaimDescriptor.EmailClaimType, Label = "Email address", IsIdentifyingClaim = true, Description = "Email identifier."},
                    new ClaimDescriptor { Type = ClaimDescriptor.DisplayNameClaimType, Label = "Display name", IsIdentifyingClaim = false, Description = "User's display name."}
                },
                Links = new LinkCollection().Add("UserSearch", "~" + LdapApi.ApiExternalUsersSearch)
            };
        }
    }
}