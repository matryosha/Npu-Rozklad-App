using System;
using Microsoft.Extensions.DependencyInjection;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Services;
using NpuRozklad.Services.Localization;

namespace NpuRozklad
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNpuCore(this IServiceCollection services,
            Action<NpuCoreOptions> optionsBuilder)
        {
            var options = new NpuCoreOptions();
            optionsBuilder(options);

            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IDayOfWeekToDateTimeConverter, DayOfWeekToDateTimeConverter>();
            services.AddSingleton<ILocalDateService, LocalDateService>();
            services.AddLocalizationLoader(options.LocalizationLoaderOptions);
            services.AddApplicationVersionProvider(options.AppVersion);

            return services;
        }

        private static void AddLocalizationLoader(this IServiceCollection services,
            Action<LocalizationLoaderOptions> localizationLoaderOptionsBuilder)
        {
            if (localizationLoaderOptionsBuilder == null)
                throw new ArgumentNullException(nameof(localizationLoaderOptionsBuilder));
            
            var options = new LocalizationLoaderOptions();
            localizationLoaderOptionsBuilder(options);

            services.AddTransient(provider => options);
            services.AddSingleton<LocalizationLoader>();
        }

        private static void AddApplicationVersionProvider(this IServiceCollection services, string appVersion)
        {
            if (string.IsNullOrWhiteSpace(appVersion))
                throw new ArgumentNullException();
            
            var provider = new ApplicationVersionProvider(appVersion);
            services.AddSingleton<IApplicationVersionProvider>(provider);
        }
    }

    public class NpuCoreOptions
    {
        public Action<LocalizationLoaderOptions> LocalizationLoaderOptions { get; set; }
        public string AppVersion { get; set; }
    }
}