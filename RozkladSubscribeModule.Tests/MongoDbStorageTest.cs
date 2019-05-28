using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure;
using RozkladSubscribeModule.Persistence;
using Xunit;

namespace RozkladSubscribeModule.Tests
{
    public class MongoDbStorageTest
    {

        private readonly string _connectionString;
        private readonly ILogger<MongoDbStorage> _mockLogger;
        private readonly IOptions<RozkladSubscribeServiceOptions> _mockOptions;


        public MongoDbStorageTest()
        {
            _connectionString = "mongodb://localhost:27017";
            _mockOptions =
                Mock.Of<IOptions<RozkladSubscribeServiceOptions>>(o =>
                    o.Value.SubscribedUsersDbConnectionString == _connectionString);
            _mockLogger =
                Mock.Of<ILogger<MongoDbStorage>>();
        }

        [Fact]
        public async Task Adding()
        {
            var databaseName = GetCallerMethodName();
            var mongoStorage = GetDefaultMongoDbStorage(databaseName);
            await mongoStorage.AddUserAsync(Helpers.CreateUsers(1, 1, "aas")
                .FirstOrDefault());
            var users = await mongoStorage.GetUsersAsync();

            Assert.Equal(1, users.Count);
            await mongoStorage.DropDatabase(databaseName);
        }

        [Fact]
        public async Task Adding2SameUsers() {

            var databaseName = GetCallerMethodName();
            var mongoStorage = GetDefaultMongoDbStorage(databaseName);
            await mongoStorage.AddUserAsync(
                new SubscribedUser(0,0, "a"));
            await mongoStorage.AddUserAsync(
                new SubscribedUser(0, 0, "a"));
            var users = await mongoStorage.GetUsersAsync();

            Assert.Equal(1, users.Count);
            await mongoStorage.DropDatabase(databaseName);

        }

        [Fact]
        public async Task Removing() {
            var databaseName = GetCallerMethodName();
            var mongoStorage = GetDefaultMongoDbStorage(databaseName);
            var user =
                new SubscribedUser(0, 0, "a");
            await mongoStorage.AddUserAsync(user);
            await mongoStorage.DeleteUserAsync(user);
            var users = await mongoStorage.GetUsersAsync();

            Assert.Equal(0, users.Count);
            await mongoStorage.DropDatabase(databaseName);
        }

        [Fact]
        public async Task RemovingNotExisted() {
            var databaseName = GetCallerMethodName();
            var mongoStorage = GetDefaultMongoDbStorage(databaseName);
            var user =
                new SubscribedUser(0, 0, "a");
            await mongoStorage.DeleteUserAsync(user);
            var users = await mongoStorage.GetUsersAsync();

            Assert.Equal(0, users.Count);
            await mongoStorage.DropDatabase(databaseName);
        }

        [Fact]
        public async Task RemovingRandom()
        {
            var databaseName = GetCallerMethodName();
            var mongoStorage = GetDefaultMongoDbStorage(databaseName);
            var user =
                new SubscribedUser(0, 0, "a");
            await mongoStorage.DeleteUserAsync(user);
            var users = await mongoStorage.GetUsersAsync();

            Assert.Equal(0, users.Count);
            await mongoStorage.DropDatabase(databaseName);
        }

        private MongoDbStorage GetDefaultMongoDbStorage(string databaseName)
        {
            return new MongoDbStorage(
                _mockLogger, 
                _mockOptions,
                databaseName,
                dropDbBeforeUse: true);
        }

        private string GetCallerMethodName([System.Runtime.CompilerServices.CallerMemberName] string memberName = "") {
            return memberName;
        }

    }
}
