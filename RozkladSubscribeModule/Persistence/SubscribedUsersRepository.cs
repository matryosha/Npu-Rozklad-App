using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Persistence
{
    internal class SubscribedUsersRepository :
        ISubscribedUsersRepository
    {
        private readonly ISubscribedUsersCache _cache;
        private readonly ISubscribedUsersPersistenceStorage _persistenceStorage;
        private readonly ILogger<SubscribedUsersRepository> _logger;

        public SubscribedUsersRepository(
            ILogger<SubscribedUsersRepository> logger,
            ISubscribedUsersCache cache,
            ISubscribedUsersPersistenceStorage persistenceStorage)
        {
            _cache = cache;
            _persistenceStorage = persistenceStorage;
            _logger = logger;


        }
        public Task AddUser(SubscribedUser subscribedUser)
        {

        }

        public Task DeleteUser(SubscribedUser seSubscribedUser)
        {
            throw new System.NotImplementedException();
        }

        public List<SubscribedUser> GetUsers()
        {
            return _cache.GetUsers();
        }

        public SubscribedUser GetUser()
        {
            throw new System.NotImplementedException();
        }

        public bool IsUserExists(SubscribedUser subscribedUser)
        {
            throw new System.NotImplementedException();
        }

    }
}