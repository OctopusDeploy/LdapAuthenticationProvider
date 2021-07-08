using System.Diagnostics.CodeAnalysis;

namespace Octopus.Server.Extensibility.Authentication.Ldap
{
    public class UserValidationResult
    {
        public UserValidationResult(UserPrincipal userPrincipal)
            : this(userPrincipal.UserPrincipalName, userPrincipal.UniqueAccountName, userPrincipal.DisplayName, userPrincipal.Email)
        {
        }

        public UserValidationResult(string? userPrincipalName, string? uniqueAccountName, string? displayName, string? emailAddress)
        {
            UserPrincipalName = userPrincipalName;
            UniqueAccountName = uniqueAccountName;
            DisplayName = displayName;
            EmailAddress = emailAddress;

            Success = true;
        }

        public UserValidationResult(string validationMessage)
        {
            ValidationMessage = validationMessage;
        }

        public string? UserPrincipalName { get; }
        public string? UniqueAccountName { get; }

        public string? DisplayName { get; }
        public string? EmailAddress { get; }

        public bool Success { get; }
        public string? ValidationMessage { get; }
    }
}