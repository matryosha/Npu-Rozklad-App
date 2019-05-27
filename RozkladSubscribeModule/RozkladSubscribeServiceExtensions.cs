using System;
using Microsoft.Extensions.DependencyInjection;
using RozkladSubscribeModuleClient.Application;
using RozkladSubscribeModuleClient.Entities;
using RozkladSubscribeModuleClient.Interfaces;

namespace RozkladSubscribeModuleClient
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