using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Web
{
    public static class ProgramExtensions
    {
        public static void RunWithNLog(this IHost host)
        {
            var services = host.Services;
            var hostEnvironment = services.GetService<IHostEnvironment>();
            var appWorkingDirectory = services.GetService<IAppWorkingDirectory>();
            
            var appVersion = services.GetService<IApplicationVersionProvider>().GetApplicationVersion();
            
            var workingDirectory = appWorkingDirectory.GetAppDirectory();
            
            var logDir = Path.Combine(workingDirectory, "logs");
            GlobalDiagnosticsContext.Set("log-dir", logDir);

            var nlogConfigFileName = !hostEnvironment.IsProduction()
                ? $"nlog.{hostEnvironment.EnvironmentName}.config"
                : "nlog.config";

            if (!File.Exists(nlogConfigFileName))
            {
                nlogConfigFileName = "nlog.default.config";
            }
            
            var logger = NLogBuilder.ConfigureNLog(nlogConfigFileName).GetCurrentClassLogger();
            logger.Log(LogLevel.Info, "Application version: {appVersion}", appVersion);
            logger.Log(LogLevel.Info, "Using {nlogConfigFileName} as nlog config", nlogConfigFileName);
            logger.Log(LogLevel.Info, "Log dir path: {logDir}", logDir);

            try
            {
                host.Run();
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e);
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}