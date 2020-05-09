using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Web
{
    public class ScopeServiceProvider : ICurrentScopeServiceProvider
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;


        public ScopeServiceProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public T GetService<T>()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var httpContext = scope.ServiceProvider.GetService<IHttpContextAccessor>().HttpContext;
            return httpContext.RequestServices.GetService<T>();
        }
    }
}