using System;
using Microsoft.Extensions.DependencyInjection;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.LessonsProvider.Fetcher;
using NpuRozklad.LessonsProvider.Holders;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLessonsProvider(this IServiceCollection services,
            Action<LessonsProviderOptions> optionsBuilder)
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }
            
            var options = new LessonsProviderOptions();
            optionsBuilder(options);
            
            services.AddNpuServerFetcher(options.FetcherOptions);
            
            services.AddSingleton<IFacultiesProvider, FacultiesProvider>();
            services.AddSingleton<IFacultyGroupsProvider, FacultyGroupsProvider>();
            services.AddSingleton<ILessonsProvider, LessonProvider>();
            
            services.AddSingleton<ICalendarRawItemHolder, CalendarRawItemHolder>();
            services.AddSingleton<IClassroomsHolder, ClassroomHolder>();
            services.AddSingleton<IGroupsHolder, GroupsHolder>();
            services.AddSingleton<ILecturersHolder, LecturersHolder>();
            services.AddSingleton<ISettingsHolder, SettingsHolder>();
            services.AddSingleton<IOddDayWeekChecker, OddDayWeekChecker>();
            services.AddSingleton<IUnprocessedExtendedLessonsHolder>(provider =>
                provider.GetService<UnprocessedExtendedLessonsManager>());
            services.AddSingleton<ICalendarRawToFacultyUnprocessedLessons>(provider =>
                provider.GetService<UnprocessedExtendedLessonsManager>());
            services.AddUnprocessedExtendedLessonsManager();

            return services;
        }

        private static void AddUnprocessedExtendedLessonsManager(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var unprocessedExtendedLessonsManager =
                ActivatorUtilities.CreateInstance<UnprocessedExtendedLessonsManager>(provider);
            
            services.AddSingleton(unprocessedExtendedLessonsManager);
        }

        private static void AddNpuServerFetcher(this IServiceCollection services,
            Action<NpuServerFetcherOptions> optionsBuilder)
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }
            
            var fetcherOptions = new NpuServerFetcherOptions();
            optionsBuilder(fetcherOptions);

            fetcherOptions.RequestOptions ??= NpuServerFetcherOptions.DefaultNpuRequestFetcherOptions();

            services.AddTransient(_ => fetcherOptions);
            services.AddSingleton<INpuServerFetcher, NpuServerFetcher>();
        }
    }

    public class LessonsProviderOptions
    {
        public Action<NpuServerFetcherOptions> FetcherOptions { get; set; }
    }
}