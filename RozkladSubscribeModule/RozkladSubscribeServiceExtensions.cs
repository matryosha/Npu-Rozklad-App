using System;
using Microsoft.Extensions.DependencyInjection;
using RozkladSubscribeModule.Application;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure;
using RozkladSubscribeModule.Interfaces;
using RozkladSubscribeModule.Persistence;

namespace RozkladSubscribeModule
{
    public static class RozkladSubscribeServiceExtensions
    {
        public static IServiceCollection AddRozkladSubscribeService(
            this IServiceCollection services,
            Action<RozkladSubscribeServiceOptions> options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (services == null) throw new ArgumentNullException(nameof(options));

            services.Configure(options);
            services.AddHostedService<SchedulerHostedService<DefaultCheckPayload, DefaultNotifyPayload>>();
            services.AddHostedService<QueuedUsersHostedService>();
            services.AddSingleton<IRozkladSubscribeService, RozkladSubscribeService>();
            services.AddSingleton<ILastSessionLessonsStorage, LastSessionLessonsStorage>();
            services.AddSingleton<ISubscribedUsersPersistenceStorage, MongoDbStorage>();
            services.AddSingleton<ISubscribedUsersCache, DefaultSubscribedUsersCache>();
            services.AddSingleton<ISubscribedUsersRepository, SubscribedUsersRepository>();
            services.AddSingleton<IBackgroundUsersQueue, BackgroundUsersQueue>();
            services
                .AddSingleton<ICheckToNotifyPayloadConverter<DefaultCheckPayload, DefaultNotifyPayload>,
                    DefaultCheckToNotifyPayloadConverter>();
            services.AddSingleton<ICheckScheduleDiffService<DefaultCheckPayload>, CheckScheduleDiffService>();
            services.AddSingleton<IDateTimesForScheduleDiffCheckGiver, DateTimesForScheduleDiffCheckGiver>();
            services.AddSingleton<ISectionLessonsManager, SectionLessonsManager>();
            services.AddSingleton<ISectionLessonsBuilder, SectionLessonsBuilder>();
            services.AddSingleton<IUserNotifyService<DefaultNotifyPayload>, TelegramNotifier>();

            return services.AddSingleton<IRozkladSubscribeService, RozkladSubscribeService>();
        }
    }
}