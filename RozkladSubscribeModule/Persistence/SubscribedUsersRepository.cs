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

//            _cache.SetUsers(_persistenceStorage.GetUsers());
        }
        public async Task AddUserAsync(SubscribedUser subscribedUser)
        {
            if (!await _cache.IsUserExistsAsync(subscribedUser))
            {

            }
        }

        public Task DeleteUserAsync(SubscribedUser seSubscribedUser)
        {
            throw new System.NotImplementedException();
        }

        public Task<ICollection<SubscribedUser>> GetUsersAsync()
        {
            return _cache.GetUsersAsync();
        }

        public SubscribedUser GetUser()
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsUserExistsAsync(SubscribedUser subscribedUser)
        {
            throw new System.NotImplementedException();
        }

    }
}