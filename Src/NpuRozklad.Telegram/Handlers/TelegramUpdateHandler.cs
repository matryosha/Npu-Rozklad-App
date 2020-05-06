using System.Threading.Tasks;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NpuRozklad.Telegram.Handlers
{
    public class TelegramUpdateHandler : ITelegramUpdateHandler
    {
        private readonly ICallbackQueryHandler _callbackQueryHandler;
        private readonly ITelegramMessageHandler _telegramMessageHandler;
        private readonly ICurrentUserInitializerService _userInitializerService;

        public TelegramUpdateHandler(
            ICallbackQueryHandler callbackQueryHandler,
            ITelegramMessageHandler telegramMessageHandler,
            ICurrentUserInitializerService userInitializerService)
        {
            _callbackQueryHandler = callbackQueryHandler;
            _telegramMessageHandler = telegramMessageHandler;
            _userInitializerService = userInitializerService;
        }

        public async Task Handle(Update update)
        {
            await _userInitializerService.InitializeCurrentUser(update);

            switch (update.Type)
            {
                case UpdateType.Message:
                    await _telegramMessageHandler.Handle(update.Message);
                    break;
                case UpdateType.CallbackQuery:
                    await _callbackQueryHandler.Handle(update.CallbackQuery);
                    break;
            }
        }
    }
}