using System;
using Microsoft.Extensions.DependencyInjection;
using RozkladSubscribeModule.Application;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule
{
    public static class RozkladSubscribeServiceExtensions
    {
        public static IServiceCollection AddRozkladSubscribeService(
            this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddHostedService<SchedulerHostedService<DefaultCheckPayload, DefaultNotifyPayload>>();
            services.AddSingleton<ISubscribedUsersManager, SubscribedUsersManager>();
            return services.AddSingleton<RozkladSubscribeService>();
        }
    }
}