using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Persistence
{
    class DefaultSubscribedUsersCache :
        ISubscribedUsersCache
    {
        private readonly ILogger<DefaultSubscribedUsersCache> _logger;

        private readonly HashSet<SubscribedUser> _cache = 
            new HashSet<SubscribedUser>();
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = 
            new ConcurrentDictionary<object, SemaphoreSlim>();


        public DefaultSubscribedUsersCache(
            ILogger<DefaultSubscribedUsersCache> logger)
        {
            _logger = logger;
        }

        public Task AddUserAsync(SubscribedUser subscribedUser)
        {
            _cache.Add(subscribedUser);
            return Task.CompletedTask;
        }

        public async Task DeleteUserAsync(SubscribedUser subscribedUser)
        {
            if (_cache.Contains(subscribedUser))
            {
                var deleteLock = _locks.GetOrAdd(
                    subscribedUser.GetHashCode(),
                    o => new SemaphoreSlim(1, 1));

                await deleteLock.WaitAsync();

                try {

                    if (_cache.Contains(subscribedUser))
                    {
                        _cache.Remove(subscribedUser);
                    }

                } finally
                {
                    deleteLock.Release();
                }
            }
        }

        public Task<ICollection<SubscribedUser>> GetUsersAsync()
        {
            return Task.FromResult<ICollection<SubscribedUser>>(
                new List<SubscribedUser>(_cache));
        }

        public Task<bool> IsUserExistsAsync(SubscribedUser subscribedUser)
        {
            return Task.FromResult(_cache.Contains(subscribedUser));
        }

        public void SetUsers(ICollection<SubscribedUser> users)
        {
            if (_cache.Any())
            {
                _logger.LogWarning("Setting cache when previous cache wasn't empty");
            }

            _cache.Clear();
            foreach (var subscribedUser in users)
            {
                _cache.Add(subscribedUser);
            }
        }
    }
}
