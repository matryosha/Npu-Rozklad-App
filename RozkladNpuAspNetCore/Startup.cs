﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Services;
using RozkladNpuAspNetCore.Utils;

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
            services.AddDbContext<BaseDbContext>(conf =>
                {
                    conf.UseMySQL(Configuration.GetSection("DbConfiguration")["ConnectionStringMySql"]);
                });
            services.AddSingleton<BotService>();
            services.AddSingleton<NpuParser>();
            services.AddTransient<MessageService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.Configure<BotConfiguration>(Configuration.GetSection("BotConfiguration"));
            services.Configure<IdkStickers>(Configuration.GetSection("Stickers"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime, BaseDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
               
            }
            context.Database.Migrate();
            app.UseMvc();
        }

    }
}
