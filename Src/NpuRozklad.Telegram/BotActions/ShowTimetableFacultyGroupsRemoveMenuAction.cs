using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Timetable;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableFacultyGroupsRemoveMenuAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly TimetableFacultyGroupsRemoveMenu _timetableFacultyGroupsRemoveMenu;
        private readonly ICurrentTelegramUserService _currentTelegramUserService;
        private readonly ILocalizationService _localizationService;

        public ShowTimetableFacultyGroupsRemoveMenuAction(
            ITelegramBotService telegramBotService,
            TimetableFacultyGroupsRemoveMenu timetableFacultyGroupsRemoveMenu,
            ICurrentTelegramUserService currentTelegramUserService,
            ILocalizationService localizationService)
        {
            _telegramBotService = telegramBotService;
            _timetableFacultyGroupsRemoveMenu = timetableFacultyGroupsRemoveMenu;
            _currentTelegramUserService = currentTelegramUserService;
            _localizationService = localizationService;
        }
        public Task Execute()
        {
            var facultyGroups = _currentTelegramUserService.TelegramRozkladUser.FacultyGroups;
            var inlineMenu = _timetableFacultyGroupsRemoveMenu.CreateInlineMenu(facultyGroups);
            var messageText = _localizationService[_currentTelegramUserService.Language, "select-group-to-remove"];

            return _telegramBotService.SendOrEditMessageAsync(
                messageText,
                replyMarkup: inlineMenu);
        }
    }
}