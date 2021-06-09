namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserValidationResult
    {
        public UserValidationResult(UserPrincipal userPrincipal)
            : this(userPrincipal.UserPrincipalName, userPrincipal.ExternalIdentity, userPrincipal.DisplayName, userPrincipal.Email)
        {
        }

        public UserValidationResult(string userPrincipalName, string externalIdentity, string displayName, string emailAddress)
        {
            UserPrincipalName = userPrincipalName;
            ExternalIdentity = externalIdentity;
            DisplayName = displayName;
            EmailAddress = emailAddress;

            Success = true;
        }

        public UserValidationResult(string validationMessage)
        {
            ValidationMessage = validationMessage;
        }

        public string UserPrincipalName { get; }
        public string ExternalIdentity { get; }

        public string DisplayName { get; }
        public string EmailAddress { get; }

        public bool Success { get; }
        public string ValidationMessage { get; }
    }
}