using System;
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
        private readonly ITelegramUserThrottle _telegramUserThrottle;

        public TelegramUpdateHandler(
            ICallbackQueryHandler callbackQueryHandler,
            ITelegramMessageHandler telegramMessageHandler,
            ICurrentUserInitializerService userInitializerService,
            ITelegramUserThrottle telegramUserThrottle)
        {
            _callbackQueryHandler = callbackQueryHandler;
            _telegramMessageHandler = telegramMessageHandler;
            _userInitializerService = userInitializerService;
            _telegramUserThrottle = telegramUserThrottle;
        }

        public async Task Handle(Update update)
        {
            try
            {
                if(_telegramUserThrottle.ShouldSkipProcessing(update)) return;
            
                await _userInitializerService.InitializeCurrentUser(update);

                switch (update.Type)
                {
                    case UpdateType.Message:
                        await _telegramMessageHandler.Handle(update.Message);
                        break;
                    case UpdateType.EditedMessage:
                        await _telegramMessageHandler.Handle(update.EditedMessage);
                        break;
                    case UpdateType.CallbackQuery:
                        await _callbackQueryHandler.Handle(update.CallbackQuery);
                        break;
                }
            }
            catch (Exception)
            {
                // Exception already has been logged so swallow it
            }
        }
    }
}