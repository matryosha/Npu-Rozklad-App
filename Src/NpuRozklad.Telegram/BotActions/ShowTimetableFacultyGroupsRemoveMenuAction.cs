using System.Threading.Tasks;
using NpuRozklad.Telegram.Display.Timetable;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableFacultyGroupsRemoveMenuAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly TimetableFacultyGroupsRemoveMenu _timetableFacultyGroupsRemoveMenu;
        private readonly ICurrentTelegramUserProvider _currentTelegramUserProvider;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public ShowTimetableFacultyGroupsRemoveMenuAction(
            ITelegramBotService telegramBotService,
            TimetableFacultyGroupsRemoveMenu timetableFacultyGroupsRemoveMenu,
            ICurrentTelegramUserProvider currentTelegramUserProvider,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _telegramBotService = telegramBotService;
            _timetableFacultyGroupsRemoveMenu = timetableFacultyGroupsRemoveMenu;
            _currentTelegramUserProvider = currentTelegramUserProvider;
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        public Task Execute()
        {
            var currentUser = _currentTelegramUserProvider.GetCurrentTelegramRozkladUser();
            var facultyGroups = currentUser.FacultyGroups;
            var inlineMenu = _timetableFacultyGroupsRemoveMenu.CreateInlineMenu(facultyGroups);
            var messageText = _currentUserLocalizationService["select-group-to-remove"];

            return _telegramBotService.SendOrEditMessageAsync(
                messageText,
                replyMarkup: inlineMenu);
        }
    }
}