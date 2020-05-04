using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Timetable.SelectingFacultyGroupToAddMenu;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableSelectingFacultyGroupToAddMenuAction
    {
        private readonly IFacultyGroupsProvider _facultyGroupsProvider;
        private readonly TimetableFacultyGroupsKeyboardCreator _keyboardCreator;
        private readonly ILocalizationService _localizationService;
        private readonly ITelegramBotService _telegramBotService;
        private readonly ICurrentTelegramUserService _currentUserService;

        public ShowTimetableSelectingFacultyGroupToAddMenuAction(IFacultyGroupsProvider facultyGroupsProvider,
            TimetableFacultyGroupsKeyboardCreator keyboardCreator,
            ILocalizationService localizationService,
            ITelegramBotService telegramBotService,
            ICurrentTelegramUserService currentUserService)
        {
            _facultyGroupsProvider = facultyGroupsProvider;
            _keyboardCreator = keyboardCreator;
            _localizationService = localizationService;
            _telegramBotService = telegramBotService;
            _currentUserService = currentUserService;
        }

        public async Task Execute(ShowTimetableSelectingFacultyGroupToAddMenuOptions options)
        {
            var facultyGroups = options.FacultyGroups;
            var messageText = _localizationService[_currentUserService.Language, "choose-group-message"];
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