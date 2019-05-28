using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Persistence
{
    class MongoDbStorage :
        ISubscribedUsersPersistenceStorage
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<SubscribedUser> _collection;
        public MongoDbStorage(
            ILogger<MongoDbStorage> logger,
            IOptions<RozkladSubscribeServiceOptions> options)
        {
            _client = new MongoClient(
                options.Value.SubscribedUsersDbConnectionString);
            _database = _client.GetDatabase("subscribed-users");
            _collection = _database.GetCollection<SubscribedUser>("users");
        }

        public Task AddUserAsync(SubscribedUser subscribedUser)
        {
            return _collection.InsertOneAsync(subscribedUser);
        }

        public Task DeleteUserAsync(SubscribedUser subscribedUser)
        {
            return _collection.DeleteOneAsync(user => user == subscribedUser);
        }

        public async Task<ICollection<SubscribedUser>> GetUsersAsync()
        {
            return await _collection.Find(u => true).ToListAsync();
        }

        public Task<bool> IsUserExistsAsync(SubscribedUser subscribedUser)
        {
            return Task.FromResult(_collection.Find(u => u == subscribedUser).Any());
        }
    }
}
