using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowIncorrectInputMessageAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentUserService;

        public ShowIncorrectInputMessageAction(ITelegramBotService telegramBotService,
            ILocalizationService localizationService,
            ICurrentTelegramUserService currentUserService
            )
        {
            _telegramBotService = telegramBotService;
            _localizationService = localizationService;
            _currentUserService = currentUserService;
        }
        
        public Task Execute(ShowIncorrectInputMessageOptions options = null)
        {
            var messageText = _localizationService[_currentUserService.Language, "incorrect-input"];

            return _telegramBotService.Client.SendTextMessageAsync(
                _currentUserService.ChatId,
                messageText);        
        }
    }

    public class ShowIncorrectInputMessageOptions
    {
        
    }
}