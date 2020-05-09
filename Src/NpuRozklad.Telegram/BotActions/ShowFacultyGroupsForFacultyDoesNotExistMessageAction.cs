using System.Threading.Tasks;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowFacultyGroupsForFacultyDoesNotExistMessageAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;
        private readonly MainMenuCreator _mainMenuCreator;

        public ShowFacultyGroupsForFacultyDoesNotExistMessageAction(ITelegramBotService telegramBotService,
            ICurrentUserLocalizationService currentUserLocalizationService,
            MainMenuCreator mainMenuCreator)
        {
            _telegramBotService = telegramBotService;
            _currentUserLocalizationService = currentUserLocalizationService;
            _mainMenuCreator = mainMenuCreator;
        }

        public Task Execute(ShowFacultyGroupsForFacultyDoesNotExistMessageOptions options = null)
        {
            var messageText = _currentUserLocalizationService["no-groups-for-faculty-message"];

            var mainMenuKeyboard = _mainMenuCreator.CreateMenu();

            return _telegramBotService.SendOrEditMessageAsync(
                messageText,
                replyMarkup: mainMenuKeyboard,
                forceNewMessage: true);
        }
    }

    public class ShowFacultyGroupsForFacultyDoesNotExistMessageOptions
    {
    }
}