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

            _cache.SetUsers(_persistenceStorage.GetUsersAsync().Result);
        }
        public async Task AddUserAsync(SubscribedUser subscribedUser)
        {
            _logger.LogDebug(
                $"Got user with id {subscribedUser.TelegramId} to add ");
            if (!await _cache.IsUserExistsAsync(subscribedUser))
            {
                _logger.LogInformation(
                    $"Adding new user with id {subscribedUser.TelegramId} to cache and persistence storage");
                await _cache.AddUserAsync(subscribedUser)
                    .ConfigureAwait(false);
                await _persistenceStorage.AddUserAsync(subscribedUser)
                    .ConfigureAwait(false);
            }
        }

        public async Task DeleteUserAsync(SubscribedUser subscribedUser)
        {
            _logger.LogDebug(
                $"Got user with id {subscribedUser.TelegramId} to delete");

            if (await _cache.IsUserExistsAsync(subscribedUser))
            {
                _logger.LogInformation(
                    $"Deleting user with id {subscribedUser.TelegramId} from cache and persistence storage");
                await _cache.DeleteUserAsync(subscribedUser)
                    .ConfigureAwait(false);
                await _persistenceStorage.DeleteUserAsync(subscribedUser)
                    .ConfigureAwait(false);
            }
        }

        public Task<ICollection<SubscribedUser>> GetUsersAsync()
        {
            _logger.LogInformation("Getting all users from cache");
            return _cache.GetUsersAsync();
        }

        public Task<bool> IsUserExistsAsync(SubscribedUser subscribedUser)
        {
            return _cache.IsUserExistsAsync(subscribedUser);
        }

    }
}