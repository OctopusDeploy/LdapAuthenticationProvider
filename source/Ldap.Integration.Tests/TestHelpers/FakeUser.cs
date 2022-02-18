using System;
using System.Collections.Generic;
using Octopus.Data.Model.User;
using Octopus.Server.Extensibility.Authentication.Ldap.Identities;
using Octopus.Server.MessageContracts.Features.Users;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal class FakeUser : IUser
    {
        public FakeUser(string userName)
            : this(userName, "", "", new HashSet<Identity> { new IdentityCreator().Create("", "", userName, "") })
        {
        }

        public FakeUser(string userName, string email, string displayName, IEnumerable<Identity> identities)
        {
            Username = userName;
            EmailAddress = email;
            DisplayName = displayName;
            Identities = new HashSet<Identity>(identities);
        }

        public string Username { get; set; }

        public Guid IdentificationToken { get; set; }
        
        public Guid SessionsToken { get; }

        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsService { get; set; }
        public bool IsActive { get; set; }

        public HashSet<Identity> Identities { get; set; }

        public UserId Id { get; set; }

        public void RevokeSessions()
        {
            throw new NotImplementedException();
        }

        public SecurityGroups GetSecurityGroups(string identityProviderName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSecurityGroups()
        {
            throw new NotImplementedException();
        }

        public void SetPassword(string plainTextPassword)
        {
            throw new NotImplementedException();
        }

        public bool ValidatePassword(string plainTextPassword)
        {
            throw new NotImplementedException();
        }
    }
}
