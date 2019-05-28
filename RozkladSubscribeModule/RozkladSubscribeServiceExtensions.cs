using System;
using Microsoft.Extensions.DependencyInjection;
using RozkladSubscribeModule.Application;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure;
using RozkladSubscribeModule.Interfaces;

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
            return services.AddSingleton<RozkladSubscribeService>();
        }
    }
}