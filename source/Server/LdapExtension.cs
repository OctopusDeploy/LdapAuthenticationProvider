using Autofac;
using Octopus.Server.Extensibility.Authentication.Extensions;
using Octopus.Server.Extensibility.Authentication.Extensions.Identities;
using Octopus.Server.Extensibility.Authentication.Ldap.Configuration;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;
using Octopus.Server.Extensibility.Authentication.Ldap.Web;
using Octopus.Server.Extensibility.Extensions;
using Octopus.Server.Extensibility.Extensions.Infrastructure;
using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using Octopus.Server.Extensibility.Extensions.Mappings;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    [OctopusPlugin("Ldap", "Octopus Deploy (contributed by Thomas Unger)")]
    public class LdapExtension : IOctopusExtension
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LdapConfigurationMapping>().As<IConfigurationDocumentMapper>().InstancePerDependency();

            builder.RegisterType<DatabaseInitializer>().As<IExecuteWhenDatabaseInitializes>().InstancePerDependency();

            builder.RegisterType<IdentityCreator>().As<IIdentityCreator>().SingleInstance();

            builder.RegisterType<LdapConfigurationStore>()
                .As<ILdapConfigurationStore>()
                .InstancePerDependency();

            builder.RegisterType<LdapConfigurationSettings>()
                .As<ILdapConfigurationSettings>()
                .As<IHasConfigurationSettings>()
                .As<IHasConfigurationSettingsResource>()
                .As<IContributeMappings>()
                .InstancePerDependency();

            builder.RegisterType<LdapUserCreationFromPrincipal>().As<ISupportsAutoUserCreationFromPrincipal>().InstancePerDependency();

            builder.RegisterType<LdapContextProvider>().As<ILdapContextProvider>().InstancePerDependency();
            builder.RegisterType<LdapObjectNameNormalizer>().As<ILdapObjectNameNormalizer>().InstancePerDependency();
            builder.RegisterType<UserPrincipalFinder>().As<IUserPrincipalFinder>().InstancePerDependency();
            builder.RegisterType<LdapExternalSecurityGroupLocator>()
                .As<ILdapExternalSecurityGroupLocator>()
                .As<ICanSearchExternalGroups>()
                .InstancePerDependency();

            builder.RegisterType<LdapService>()
                .As<ILdapService>()
                .InstancePerDependency();

            builder.RegisterType<LdapCredentialValidator>()
                .As<ILdapCredentialValidator>()
                .As<IDoesBasicAuthentication>()
                .InstancePerDependency();

            builder.RegisterType<GroupRetriever>()
                .As<IExternalGroupRetriever>()
                .InstancePerDependency();

            builder.RegisterType<UserSearch>().As<ICanSearchExternalUsers>().As<ICanSearchLdapUsers>().InstancePerDependency();
            builder.RegisterType<UserMatcher>().As<ICanMatchExternalUser>().InstancePerDependency();

            builder.RegisterType<LdapConfigureCommands>()
                .As<IContributeToConfigureCommand>()
                .InstancePerDependency();

            builder.RegisterType<ListSecurityGroupsAction>().AsSelf().InstancePerDependency();
            builder.RegisterType<UserLookupAction>().AsSelf().InstancePerDependency();

            builder.RegisterType<LdapAuthenticationProvider>()
                .As<IAuthenticationProvider>()
                .As<IAuthenticationProviderWithGroupSupport>()
                .As<IUseAuthenticationIdentities>()
                .AsSelf()
                .InstancePerDependency();
        }
    }
}