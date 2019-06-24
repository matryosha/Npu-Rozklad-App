using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using RozkladNpuBot.Persistence.Helpers;

namespace RozkladNpuBot.WebApi
{
    public class Program
    {
        public static void Main(string[] args) {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            ConfigureConfiguration(configurationBuilder);
            IConfiguration configuration = configurationBuilder.Build();
            var mysqlConnectionString = configuration.GetSection("DbConfiguration")["ConnectionStringMySql"];
            var mongoConnectionString = configuration.GetSection("DbConfiguration")["ConnectionStringMongoDb"];

            CheckDatabaseConnections.CheckDbConnections(mysqlConnectionString, 
                mongoConnectionString);
            CreateWebHostBuilder(args).Build().Run();
            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigureConfiguration)
                .UseStartup<Startup>();
        }
        public static void ConfigureConfiguration(IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "Properties/secret.json"),
                optional: false, reloadOnChange: true);
            config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(),
                    $"Properties/secret.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json"),
                optional: true, reloadOnChange: true);
        }
    }
}