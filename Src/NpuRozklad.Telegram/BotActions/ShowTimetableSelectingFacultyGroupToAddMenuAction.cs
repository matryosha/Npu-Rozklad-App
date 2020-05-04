using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Display.Timetable.SelectingFacultyGroupToAddMenu;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableSelectingFacultyGroupToAddMenuAction
    {
        private readonly TimetableFacultyGroupsKeyboardCreator _keyboardCreator;
        private readonly ITelegramBotService _telegramBotService;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public ShowTimetableSelectingFacultyGroupToAddMenuAction(
            TimetableFacultyGroupsKeyboardCreator keyboardCreator,
            ITelegramBotService telegramBotService,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _keyboardCreator = keyboardCreator;
            _telegramBotService = telegramBotService;
            _currentUserLocalizationService = currentUserLocalizationService;
        }

        public async Task Execute(ShowTimetableSelectingFacultyGroupToAddMenuOptions options)
        {
            var facultyGroups = options.FacultyGroups;
            var messageText = _currentUserLocalizationService["choose-group-message"];
            var keyboardMarkup = _keyboardCreator.CreateMarkup(new TimetableFacultyGroupsKeyboardOptions
                {FacultyGroups = facultyGroups});

            await _telegramBotService.SendOrEditMessageAsync(
                messageText,
                replyMarkup: keyboardMarkup,
                forceNewMessage: true);
        }
    }
    
    public class ShowTimetableSelectingFacultyGroupToAddMenuOptions
    {
        public ICollection<Group> FacultyGroups { get; set; }
    }
}