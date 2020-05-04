using NpuRozklad.Telegram.Interfaces;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Services
{
    public class CurrentTelegramUserProvider : ICurrentTelegramUserProvider
    {
        private readonly ICurrentScopeServiceProvider _currentScopeServiceProvider;

        public CurrentTelegramUserProvider(ICurrentScopeServiceProvider currentScopeServiceProvider )
        {
            _currentScopeServiceProvider = currentScopeServiceProvider;
        }

        public TelegramRozkladUser GetCurrentTelegramRozkladUser() => _currentScopeServiceProvider
            .GetService<ICurrentTelegramUserContext>()?.TelegramRozkladUser;
    }
}