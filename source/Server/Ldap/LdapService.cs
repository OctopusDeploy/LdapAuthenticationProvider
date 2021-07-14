using Octopus.Diagnostics;
using System;
using System.Threading;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class LdapService : ILdapService
    {
        readonly ILog log;
        readonly ILdapObjectNameNormalizer objectNameNormalizer;
        readonly ILdapContextProvider contextProvider;
        readonly IUserPrincipalFinder userPrincipalFinder;

        public LdapService(ILog log,
            ILdapObjectNameNormalizer objectNameNormalizer,
            ILdapContextProvider contextProvider,
            IUserPrincipalFinder userPrincipalFinder)
        {
            this.log = log;
            this.objectNameNormalizer = objectNameNormalizer;
            this.contextProvider = contextProvider;
            this.userPrincipalFinder = userPrincipalFinder;
        }

        public UserValidationResult ValidateCredentials(string username, string password, CancellationToken cancellationToken)
        {
            log.Verbose($"Validating credentials provided for '{username}'...");

            objectNameNormalizer.NormalizeName(username, out username, out var domain);

            using var context = contextProvider.GetContext();
            var uniqueAccountName = objectNameNormalizer.BuildUserName(username, domain);
            var principal = userPrincipalFinder.FindByIdentity(context, uniqueAccountName);

            if (principal == null)
            {
                var searchedContext = domain ?? context.UserBaseDN;
                log.Info($"A principal identifiable by '{uniqueAccountName}' was not found in '{searchedContext}'");

                return username.Contains("@")
                    ? new UserValidationResult("Invalid username or password.  UPN format may not be supported for your domain configuration.")
                    : new UserValidationResult("Invalid username or password.");
            }

            try
            {
                log.Verbose($"Calling Bind ('{principal.DistinguishedName}')");
                context.LdapConnection.Bind(principal.DistinguishedName, password);
                log.Verbose($"Credentials for '{uniqueAccountName}' validated, mapped to principal '{principal.UniqueAccountName}'");
            }
            catch (Exception)
            {
                log.Warn($"Principal '{principal.DistinguishedName}' (Domain: '{domain}') could not be logged on via LDAP.");
                return new UserValidationResult("Invalid username or password.");
            }

            return new UserValidationResult(principal);
        }


        public UserValidationResult FindByIdentity(string username)
        {
            objectNameNormalizer.NormalizeName(username, out username, out var domain);

            using var context = contextProvider.GetContext();
            var uniqueAccountName = objectNameNormalizer.BuildUserName(username, domain);
            var principal = userPrincipalFinder.FindByIdentity(context, uniqueAccountName);

            if (principal != null)
                return new UserValidationResult(principal);

            var searchedContext = domain ?? context.UserBaseDN;
            return new UserValidationResult($"A principal identifiable by '{uniqueAccountName}' was not found in '{searchedContext}'");
        }
    }
}