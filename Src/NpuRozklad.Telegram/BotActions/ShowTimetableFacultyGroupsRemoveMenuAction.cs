using System.Threading.Tasks;
using NpuRozklad.Telegram.Display.Timetable;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableFacultyGroupsRemoveMenuAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly TimetableFacultyGroupsRemoveMenu _timetableFacultyGroupsRemoveMenu;
        private readonly ICurrentTelegramUserContext _currentTelegramUserContext;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public ShowTimetableFacultyGroupsRemoveMenuAction(
            ITelegramBotService telegramBotService,
            TimetableFacultyGroupsRemoveMenu timetableFacultyGroupsRemoveMenu,
            ICurrentTelegramUserContext currentTelegramUserContext,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _telegramBotService = telegramBotService;
            _timetableFacultyGroupsRemoveMenu = timetableFacultyGroupsRemoveMenu;
            _currentTelegramUserContext = currentTelegramUserContext;
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        public Task Execute()
        {
            var facultyGroups = _currentTelegramUserContext.TelegramRozkladUser.FacultyGroups;
            var inlineMenu = _timetableFacultyGroupsRemoveMenu.CreateInlineMenu(facultyGroups);
            var messageText = _currentUserLocalizationService["select-group-to-remove"];

            return _telegramBotService.SendOrEditMessageAsync(
                messageText,
                replyMarkup: inlineMenu);
        }
    }
}