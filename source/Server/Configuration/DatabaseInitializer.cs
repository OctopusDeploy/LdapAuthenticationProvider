using Octopus.Data.Storage.Configuration;
using Octopus.Diagnostics;
using Octopus.Server.Extensibility.Extensions.Infrastructure;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Configuration
{
    public class DatabaseInitializer : ExecuteWhenDatabaseInitializes
    {
        private readonly ISystemLog log;
        private readonly IConfigurationStore configurationStore;

        public DatabaseInitializer(ISystemLog log, IConfigurationStore configurationStore)
        {
            this.log = log;
            this.configurationStore = configurationStore;
        }

        public override void Execute()
        {
            var doc = configurationStore.Get<LdapConfiguration>(LdapConfigurationStore.SingletonId);
            if (doc != null)
                return;

            log.Info("Initializing LDAP configuration settings");
            doc = new LdapConfiguration();
            configurationStore.Create(doc);
        }
    }
}