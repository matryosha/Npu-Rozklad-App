using System.Threading.Tasks;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowMainMenuAction
    {
        private readonly MainMenuCreator _mainMenuCreator;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;
        private readonly ITelegramBotService _telegramBotService;

        public ShowMainMenuAction(MainMenuCreator mainMenuCreator,
            ICurrentUserLocalizationService currentUserLocalizationService,
            ITelegramBotService telegramBotService)
        {
            _mainMenuCreator = mainMenuCreator;
            _currentUserLocalizationService = currentUserLocalizationService;
            _telegramBotService = telegramBotService;
        }

        public Task Execute(ShowMainMenuOptions options = null)
        {
            var replyKeyboard = _mainMenuCreator.CreateMenu();
            var messageText = GetMessageText(options);

            return _telegramBotService.SendOrEditMessageAsync(
                messageText,
                replyMarkup: replyKeyboard,
                forceNewMessage: true);
        }

        private string GetMessageText(ShowMainMenuOptions options)
        {
            var optionLocalizationValue = options?.LocalizationValueToShow;
            
            return string.IsNullOrWhiteSpace(optionLocalizationValue)
                ? MessageDefaultText()
                : _currentUserLocalizationService[optionLocalizationValue];
        }

        private string MessageDefaultText() =>
            _currentUserLocalizationService["choose-action-message"];
    }

    public class ShowMainMenuOptions
    {
        public string LocalizationValueToShow { get; set; }
    }
}