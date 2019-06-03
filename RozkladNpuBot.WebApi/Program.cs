using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace RozkladNpuBot.WebApi
{
    public class Program
    {
        public static void Main(string[] args) {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hosting, config) => {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "Properties/secret.json"),
                        optional: false, reloadOnChange: true);
                    config.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(),
                            $"Properties/secret.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json"),
                        optional: true, reloadOnChange: true);
                })
                .UseUrls("http://*:1616")
                .UseStartup<Startup>();
        }
    }
}