using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowFacultyGroupsForFacultyDoesNotExistMessageAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentUserService;
        private readonly MainMenuCreator _mainMenuCreator;

        public ShowFacultyGroupsForFacultyDoesNotExistMessageAction(ITelegramBotService telegramBotService,
            ILocalizationService localizationService,
            ICurrentTelegramUserService currentUserService, 
            MainMenuCreator mainMenuCreator)
        {
            _telegramBotService = telegramBotService;
            _localizationService = localizationService;
            _currentUserService = currentUserService;
            _mainMenuCreator = mainMenuCreator;
        }

        public Task Execute(ShowFacultyGroupsForFacultyDoesNotExistMessageOptions options = null)
        {
            var messageText = _localizationService[_currentUserService.Language, "no-groups-for-faculty-message"];

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