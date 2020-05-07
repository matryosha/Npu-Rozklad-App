using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCorePersistence(this IServiceCollection services,
            Action<CorePersistenceOptions> optionsBuilder)
        {
            if (optionsBuilder == null)
                throw new ArgumentNullException(nameof(optionsBuilder));
            
            var options = new CorePersistenceOptions();
            optionsBuilder(options);

            services.AddScoped<IRozkladUsersDao, RozkladUsersDao>();
            services.AddNpuRozkladContext(options);

            return services;
        }

        private static void AddNpuRozkladContext(this IServiceCollection services,
            CorePersistenceOptions options)
        {
            if (options.UseInMemoryDb)
            {
                services.AddDbContext<NpuRozkladContext>(builder => 
                    builder.UseInMemoryDatabase("npurozklad"));
            }
            else
            {
                services.AddDbContext<NpuRozkladContext>(builder => 
                    builder.UseMySql(options.ConnectionString, optionsBuilder => 
                        optionsBuilder
                            .EnableRetryOnFailure(10)
                            .MigrationsHistoryTable("core-migrations")));
                var dbContext = services.BuildServiceProvider().GetService<NpuRozkladContext>();
                dbContext.Database.Migrate();
            }
        }
    }

    public class CorePersistenceOptions
    {
        public string ConnectionString { get; set; }
        public bool UseInMemoryDb { get; set; }
    }
}