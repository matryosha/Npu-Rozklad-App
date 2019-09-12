using RozkladNpuBot.Application.Interfaces;

namespace RozkladNpuBot.WebApi
{
    public class WebApiApplicationGiver : IApplicationVersionGiver
    {
        public string GetApplicationVersion()
        {
            return typeof(Startup).Assembly.GetName().Version.ToString();
        }
    }
}