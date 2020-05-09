using System;
using Microsoft.Extensions.Hosting;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Web
{
    public class AppWorkingDirectoryProvider : IAppWorkingDirectory
    {
        private readonly IHostEnvironment _environment;

        public AppWorkingDirectoryProvider(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public string GetAppDirectory()
        {
            return _environment.IsDevelopment() 
                ? AppDomain.CurrentDomain.BaseDirectory 
                : _environment.ContentRootPath;
        }
    }
}