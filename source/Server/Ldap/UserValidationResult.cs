namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserValidationResult
    {
        public UserValidationResult(UserPrincipal userPrincipal)
            : this(userPrincipal.UPN, userPrincipal.SamAccountName, userPrincipal.DisplayName, userPrincipal.Mail)
        {
        }

        public UserValidationResult(string userPrincipalName, string samAccountName, string displayName, string emailAddress)
        {
            UserPrincipalName = userPrincipalName;
            SamAccountName = samAccountName;
            DisplayName = displayName;
            EmailAddress = emailAddress;

            Success = true;
        }

        public UserValidationResult(string validationMessage)
        {
            ValidationMessage = validationMessage;
        }

        public string UserPrincipalName { get; }
        public string SamAccountName { get; }

        public string DisplayName { get; }
        public string EmailAddress { get; }

        public bool Success { get; }
        public string ValidationMessage { get; }
    }
}