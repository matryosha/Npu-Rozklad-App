using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Services
{
    public class ApplicationVersionProvider : IApplicationVersionProvider
    {
        private readonly string _appVersion;
        
        public ApplicationVersionProvider(string appVersion)
        {
            _appVersion = appVersion;
        }

        public string GetApplicationVersion() => _appVersion;
    }
}