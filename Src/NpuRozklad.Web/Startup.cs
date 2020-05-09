using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.LessonsProvider;
using NpuRozklad.Persistence;
using NpuRozklad.Telegram;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["MySqlConnectionString"];
            
            services.AddControllers().AddNewtonsoftJson();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSingleton<ICurrentScopeServiceProvider, ScopeServiceProvider>();
            services.AddSingleton<IAppWorkingDirectory, AppWorkingDirectoryProvider>();

            services.AddNpuCore(npuOptions =>
            {
                npuOptions.AppVersion = Configuration["AppVersion"];
                npuOptions.LocalizationLoaderOptions = loaderOptions =>
                {
                    loaderOptions.PathToLocalizationsFiles =
                        Path.Combine("Properties", "localizations");
                };
            });
            services.AddCorePersistence(coreOptions =>
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                    coreOptions.UseInMemoryDb = true;
                else
                    coreOptions.ConnectionString = connectionString;

            });
            services.AddLessonsProvider(providerOptions =>
            {
                providerOptions.FetcherOptions = fetcherOptions =>
                {
                    fetcherOptions.BaseAddress = Configuration["LessonsProvider:BaseAddress"];
                    fetcherOptions.CallEndPoint = Configuration["LessonsProvider:CallEndPoint"];
                };
            });
            services.AddTelegramNpu(telegramOptions =>
            {
                telegramOptions.BotApiToken = Configuration["BotToken"];
                if(string.IsNullOrWhiteSpace(connectionString))
                    telegramOptions.UseInMemoryDb = true;
                else
                    telegramOptions.ConnectionString = connectionString;
            });
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}