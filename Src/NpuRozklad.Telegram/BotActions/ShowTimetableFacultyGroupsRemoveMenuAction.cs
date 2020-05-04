using System.Threading.Tasks;
using NpuRozklad.Telegram.Display.Timetable;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableFacultyGroupsRemoveMenuAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly TimetableFacultyGroupsRemoveMenu _timetableFacultyGroupsRemoveMenu;
        private readonly ICurrentTelegramUserService _currentTelegramUserService;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public ShowTimetableFacultyGroupsRemoveMenuAction(
            ITelegramBotService telegramBotService,
            TimetableFacultyGroupsRemoveMenu timetableFacultyGroupsRemoveMenu,
            ICurrentTelegramUserService currentTelegramUserService,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _telegramBotService = telegramBotService;
            _timetableFacultyGroupsRemoveMenu = timetableFacultyGroupsRemoveMenu;
            _currentTelegramUserService = currentTelegramUserService;
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        public Task Execute()
        {
            var facultyGroups = _currentTelegramUserService.TelegramRozkladUser.FacultyGroups;
            var inlineMenu = _timetableFacultyGroupsRemoveMenu.CreateInlineMenu(facultyGroups);
            var messageText = _currentUserLocalizationService["select-group-to-remove"];

            return _telegramBotService.SendOrEditMessageAsync(
                messageText,
                replyMarkup: inlineMenu);
        }
    }
}