using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RozkladNpuAspNetCore.Configurations;
using RozkladNpuAspNetCore.Infrastructure;
using RozkladNpuAspNetCore.Interfaces;
using RozkladNpuAspNetCore.Persistence;
using RozkladNpuAspNetCore.Services;

namespace RozkladNpuAspNetCore
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
            services.AddDbContext<RozkladBotContext>(conf =>
                {
                    conf.UseMySql(Configuration.GetSection("DbConfiguration")["ConnectionStringMySql"]);
                });
            services.AddSingleton<BotService>();
            services.AddSingleton<ILessonsProvider, NpuLessonsProvider>();
            services.AddScoped<IInlineKeyboardReplyService, InlineKeyboardReplyService>();
            services.AddScoped<IKeyboardReplyService, KeyboardReplyService>();
            services.AddSingleton<IUserService, DatabaseOnlyUserService>();
            services.AddScoped<IMessageHandleService, MessageHandleService>();
            services.AddScoped<ICallbackQueryHandlerService, CallbackQueryHandlerService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<BotConfiguration>(options => Configuration.GetSection("BotConfiguration").Bind(options));
            services.Configure<UnknownResponseConfiguration>(Configuration.GetSection("Stickers"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,  RozkladBotContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
               
            }
            context.Database.Migrate();
            app.Use(async (httpContext, next) =>
            {
                await next();
                var path = httpContext.Request.Path.Value;

                if (!path.StartsWith("/api") && !path.StartsWith("/update") && !Path.HasExtension(path))
                {
                    httpContext.Request.Path = "/index.html";
                    await next();
                }
            });
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
            
        }

    }
}
