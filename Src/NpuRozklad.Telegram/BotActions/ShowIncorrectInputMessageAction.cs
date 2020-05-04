using System.Threading.Tasks;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowIncorrectInputMessageAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public ShowIncorrectInputMessageAction(ITelegramBotService telegramBotService,
            ICurrentUserLocalizationService currentUserLocalizationService
            )
        {
            _telegramBotService = telegramBotService;
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        
        public Task Execute(ShowIncorrectInputMessageOptions options = null)
        {
            var messageText = _currentUserLocalizationService["incorrect-input"];

            return _telegramBotService.SendOrEditMessageAsync(
                messageText,
                forceNewMessage: true);        
        }
    }

    public class ShowIncorrectInputMessageOptions
    {
        
    }
}