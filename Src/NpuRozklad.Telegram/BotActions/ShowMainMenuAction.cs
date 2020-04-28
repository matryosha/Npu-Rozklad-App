using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowMainMenuAction
    {
        private readonly MainMenuCreator _mainMenuCreator;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentTelegramUserService;
        private readonly ITelegramBotService _telegramBotService;

        public ShowMainMenuAction(MainMenuCreator mainMenuCreator,
            ILocalizationService localizationService,
            ICurrentTelegramUserService currentTelegramUserService,
            ITelegramBotService telegramBotService)
        {
            _mainMenuCreator = mainMenuCreator;
            _localizationService = localizationService;
            _currentTelegramUserService = currentTelegramUserService;
            _telegramBotService = telegramBotService;
        }

        public Task Execute(ShowMainMenuOptions options = null)
        {
            var replyKeyboard = _mainMenuCreator.CreateMenu();
            var messageText = GetMessageText(options);

            return _telegramBotService.Client.SendTextMessageAsync(
                _currentTelegramUserService.ChatId,
                messageText,
                replyMarkup: replyKeyboard);
        }

        private string GetMessageText(ShowMainMenuOptions options)
        {
            var optionLocalizationValue = options?.LocalizationValueToShow;
            
            return string.IsNullOrWhiteSpace(optionLocalizationValue)
                ? MessageDefaultText()
                : _localizationService[_currentTelegramUserService.Language, optionLocalizationValue];
        }

        private string MessageDefaultText() =>
            _localizationService[_currentTelegramUserService.Language, "choose-action-message"];
    }

    public class ShowMainMenuOptions
    {
        public string LocalizationValueToShow { get; set; }
    }
}