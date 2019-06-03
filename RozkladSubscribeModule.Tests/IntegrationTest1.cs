using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;
using NpuTimetableParser;
using RozkladNpuBot.Domain.Entities;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure;
using RozkladSubscribeModule.Interfaces;
using RozkladSubscribeModule.Persistence;
using Xunit;

namespace RozkladSubscribeModule.Tests
{

    public class IntegrationTest1
    {
        private readonly ContainerBuilder _builder;
        private readonly IContainer _container;

        public IntegrationTest1()
        {
            _builder = new ContainerBuilder();
            _builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            _builder.RegisterType(typeof(LoggerFactory))
                .As(typeof(ILoggerFactory))
                .SingleInstance();

            _builder.RegisterType(typeof(DebugLoggerProvider))
                .As(typeof(ILoggerProvider))
                .InstancePerDependency();

            _builder.RegisterType(typeof(RozkladSubscribeService))
                .As(typeof(IRozkladSubscribeService))
                .SingleInstance();

            _builder.RegisterType(typeof(BackgroundUsersQueue))
                .As(typeof(IBackgroundUsersQueue))
                .SingleInstance();

            

            var options = new TestOptions(new RozkladSubscribeServiceOptions {
                SubscribedUsersDbConnectionString = "mongodb://localhost:27017",
                CheckTimeType = CheckTimeType.LastDaysOfCurrentWeek
            });

            _builder.RegisterType(typeof(MongoDbStorage))
                .As(typeof(ISubscribedUsersPersistenceStorage))
                .WithParameter(new NamedParameter("databaseName", "integr-test-1"))
                .WithParameter(new NamedParameter("collectionName", "users"))
                .WithParameter(new NamedParameter("dropDbBeforeUse", true))
                .SingleInstance();


            _builder.RegisterType(typeof(SubscribedUsersRepository))
                .As(typeof(ISubscribedUsersRepository))
                .SingleInstance();


            _builder.RegisterInstance(options)
                .As<IOptions<RozkladSubscribeServiceOptions>>()
                .SingleInstance();

            _builder.RegisterType(typeof(DefaultSubscribedUsersCache))
                .As(typeof(ISubscribedUsersCache))
                .SingleInstance();

            _builder.RegisterType(typeof(QueuedUsersHostedService))
                .SingleInstance();



            _container = _builder.Build();


        }

        [Fact]
        public async Task UserSubscribedAndCanBeUsed()
        {
            var rozkladSubscribeService = _container.Resolve<IRozkladSubscribeService>();
            var queuedUsersService = _container.Resolve<QueuedUsersHostedService>();
            var cache = _container.Resolve<ISubscribedUsersCache>();
            var repo = _container.Resolve<ISubscribedUsersRepository>();
            Task.Run(() => queuedUsersService.StartAsync(CancellationToken.None));
            var tasks = new List<Task>();
            var users = Helpers.CreateUsers(10, 2, "asd");
            users.ForEach(user =>
            {
                var task = new Task(() =>
                {
                    rozkladSubscribeService.SubscribeUser(new RozkladUser
                        {
                            TelegramId = user.TelegramId
                        },
                        user.ChatId,
                        new Group
                        {
                            FacultyShortName = user.FacultyShortName,
                            ExternalId = user.GroupExternalId
                        });
                });

                tasks.Add(task);
            });

            foreach (var task in tasks)
            {
                task.Start();
            }

            await Task.WhenAll(tasks.ToArray());
            await Task.Delay(1000);

            var repoUsers = await repo.GetUsersAsync();
            Assert.Equal(10, repoUsers.Count);
        }

        private class TestOptions :
            IOptions<RozkladSubscribeServiceOptions>
        {
            public TestOptions(RozkladSubscribeServiceOptions options)
            {
                Value = options;
            }

            public RozkladSubscribeServiceOptions Value { get; }
        }
    }
}