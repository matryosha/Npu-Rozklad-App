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
            services.AddNpuRozkladContext(options.ConnectionString);

            return services;
        }

        private static void AddNpuRozkladContext(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<NpuRozkladContext>(builder => 
                builder.UseMySql(connectionString, optionsBuilder => 
                    optionsBuilder.EnableRetryOnFailure(10)));
            var dbContext = services.BuildServiceProvider().GetService<NpuRozkladContext>();
            dbContext.Database.Migrate();
        }
    }

    public class CorePersistenceOptions
    {
        public string ConnectionString { get; set; }
    }
}