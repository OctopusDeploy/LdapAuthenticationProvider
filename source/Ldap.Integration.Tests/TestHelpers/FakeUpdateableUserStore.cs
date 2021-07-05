using System;
using System.Collections.Generic;
using System.Threading;
using Octopus.Data;
using Octopus.Data.Model.User;
using Octopus.Data.Storage.User;
using Octopus.Server.Extensibility.Authentication.HostServices;
using Octopus.Server.MessageContracts.Features.Users;

namespace Ldap.Integration.Tests.TestHelpers
{
    internal class FakeUpdateableUserStore : IUpdateableUserStore
    {
        public IUser AddIdentity(UserId userId, Identity identity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void ClearSecurityGroupIds(string provider, UserId userId)
        {
            throw new NotImplementedException();
        }

        public IResult<IUser> Create(string username, string displayName, string emailAddress, CancellationToken cancellationToken, ProviderUserGroups providerGroups = null, IEnumerable<Identity> identities = null, ApiKeyDescriptor apiKeyDescriptor = null, bool isService = false, string password = null)
        {
            // Pretend we succesfully created a user
            return Result<IUser>.Success(new FakeUser(username, emailAddress, displayName, identities));
        }

        public void DisableUser(UserId userId)
        {
            throw new NotImplementedException();
        }

        public void EnableUser(UserId userId)
        {
            throw new NotImplementedException();
        }

        public IUser[] GetByEmailAddress(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public IUser GetById(string userId)
        {
            throw new NotImplementedException();
        }

        public IUser GetByIdentificationToken(Guid identificationToken)
        {
            throw new NotImplementedException();
        }

        public IUser[] GetByIdentity(Identity identityToMatch)
        {
            // Pretend we have no users
            return new IUser[0];
        }

        public IUser GetByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public void SetSecurityGroupIds(string provider, UserId userId, IEnumerable<string> ids, DateTimeOffset updated)
        {
            throw new NotImplementedException();
        }

        public IUser UpdateIdentity(UserId userId, Identity identity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
