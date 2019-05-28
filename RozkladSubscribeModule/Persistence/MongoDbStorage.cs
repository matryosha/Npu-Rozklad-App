using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure;
using RozkladSubscribeModule.Interfaces;
using RozkladSubscribeModule.Persistence.Extensions;

namespace RozkladSubscribeModule.Persistence
{
    class MongoDbStorage :
        ISubscribedUsersPersistenceStorage
    {
        private readonly ILogger<MongoDbStorage> _logger;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<MongoSubscribedUser> _collection;

        static MongoDbStorage()
        {
            BsonClassMap.RegisterClassMap<MongoSubscribedUser>(msu =>
                {
                    msu.AutoMap();
                    msu.MapIdField(f => f.Id);
                }
            );
        }

        public MongoDbStorage(
            ILogger<MongoDbStorage> logger,
            IOptions<RozkladSubscribeServiceOptions> options,
            string databaseName = "subscribed-users",
            string collectionName = "users",
            bool dropDbBeforeUse = false)
        {
            _logger = logger;
            _client = new MongoClient(
                options.Value.SubscribedUsersDbConnectionString);
            if (dropDbBeforeUse)
                _client.DropDatabase(databaseName);
            _database = _client.GetDatabase(databaseName);
            
            _collection = _database.GetCollection<MongoSubscribedUser>(collectionName);
        }

        public async Task AddUserAsync(SubscribedUser subscribedUser)
        {
            try
            {
                await _collection.InsertOneAsync(subscribedUser.ToMongoSubscribedUser())
                    .ConfigureAwait(false);
            }
            catch (MongoWriteException e)
            {
                var innerExceptionMessage = e.InnerException.Message;
                if (Regex.Match(innerExceptionMessage, "E11000").Success)
                    _logger.LogWarning("Adding subscribed user that already present in database");
                else
                    throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception when trying add subscribed user");
            }
        }

        public Task DeleteUserAsync(SubscribedUser subscribedUser)
        {
            return _collection.DeleteOneAsync(user => user.Id == subscribedUser.GetHashCode());
        }

        public async Task<ICollection<SubscribedUser>> GetUsersAsync()
        {
            var mongoUsers = await _collection.Find(u => true).ToListAsync();
            return mongoUsers.ToSubscribedUsers();
        }

        public Task<bool> IsUserExistsAsync(SubscribedUser subscribedUser)
        {
            return Task.FromResult(_collection.Find(u => u.Id == subscribedUser.GetHashCode()).Any());
        }

        public Task DropDatabase(string databaseName)
        {
            return _client.DropDatabaseAsync(databaseName);
        }
    }
}
