using Octopus.Server.Extensibility.Extensions.Infrastructure.Configuration;
using System;

namespace Octopus.Server.Extensibility.Authentication.Ldap.Configuration
{
    public class LdapConfigurationMapping : IConfigurationDocumentMapper
    {
        public Type GetTypeToMap() => typeof(LdapConfiguration);
    }
}