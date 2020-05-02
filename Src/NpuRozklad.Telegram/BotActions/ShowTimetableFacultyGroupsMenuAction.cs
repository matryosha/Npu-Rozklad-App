using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Timetable;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.Enums;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableFacultyGroupsMenuAction 
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly TimetableFacultyGroupsMenu _timetableFacultyGroupsMenu;
        private readonly ICurrentTelegramUserService _currentTelegramUserService;
        private readonly ILocalizationService _localizationService;

        public ShowTimetableFacultyGroupsMenuAction(
            ITelegramBotService telegramBotService,
            TimetableFacultyGroupsMenu timetableFacultyGroupsMenu,
            ICurrentTelegramUserService currentTelegramUserService,
            ILocalizationService localizationService)
        {
            _telegramBotService = telegramBotService;
            _timetableFacultyGroupsMenu = timetableFacultyGroupsMenu;
            _currentTelegramUserService = currentTelegramUserService;
            _localizationService = localizationService;
        }

        public Task Execute(ShowTimetableFacultyGroupsMenuOptions options)
        {
            var facultyGroups = GetFacultyGroups(options);
            var inlineMenu = _timetableFacultyGroupsMenu.CreateInlineMenu(facultyGroups);
            var textMessage = _localizationService[_currentTelegramUserService.Language, "choose-group-message"];

            return _telegramBotService.Client.SendTextMessageAsync(
                _currentTelegramUserService.ChatId,
                textMessage,
                ParseMode.Markdown,
                replyMarkup: inlineMenu
            );
        }

        private ICollection<Group> GetFacultyGroups(ShowTimetableFacultyGroupsMenuOptions options)
        {
            var facultyGroups = options?.FacultyGroups;
            return facultyGroups ?? _currentTelegramUserService.TelegramRozkladUser.FacultyGroups;
        }
    }
    
    public class ShowTimetableFacultyGroupsMenuOptions
    {
        public ICollection<Group> FacultyGroups { get; set; }
    }
}