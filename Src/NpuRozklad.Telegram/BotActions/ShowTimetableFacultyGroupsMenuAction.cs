using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Display.Timetable;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.Enums;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableFacultyGroupsMenuAction 
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly TimetableFacultyGroupsMenu _timetableFacultyGroupsMenu;
        private readonly ICurrentTelegramUserProvider _currentTelegramUserProvider;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public ShowTimetableFacultyGroupsMenuAction(
            ITelegramBotService telegramBotService,
            TimetableFacultyGroupsMenu timetableFacultyGroupsMenu,
            ICurrentTelegramUserProvider currentTelegramUserProvider,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _telegramBotService = telegramBotService;
            _timetableFacultyGroupsMenu = timetableFacultyGroupsMenu;
            _currentTelegramUserProvider = currentTelegramUserProvider;
            _currentUserLocalizationService = currentUserLocalizationService;
        }

        public Task Execute(ShowTimetableFacultyGroupsMenuOptions options)
        {
            var facultyGroups = GetFacultyGroups(options);
            var inlineMenu = _timetableFacultyGroupsMenu.CreateInlineMenu(facultyGroups);
            var textMessage = _currentUserLocalizationService["select-group-to-show-timetable"];

            return _telegramBotService.SendOrEditMessageAsync(
                textMessage,
                ParseMode.Markdown,
                replyMarkup: inlineMenu
            );
        }

        private ICollection<Group> GetFacultyGroups(ShowTimetableFacultyGroupsMenuOptions options)
        {
            var facultyGroups = options?.FacultyGroups;
            return facultyGroups ?? _currentTelegramUserProvider.GetCurrentTelegramRozkladUser().FacultyGroups;
        }
    }
    
    public class ShowTimetableFacultyGroupsMenuOptions
    {
        public ICollection<Group> FacultyGroups { get; set; }
    }
}