using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Timetable.SelectingFacultyMenu;
using NpuRozklad.Telegram.LongLastingUserActions;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableSelectingFacultyMenuAction
    {
        private readonly TimetableFacultyListKeyboardCreator _keyboardCreator;
        private readonly IFacultiesProvider _facultiesProvider;
        private readonly ITelegramBotService _telegramBotService;
        private readonly ICurrentTelegramUserService _currentUserService;
        private readonly ILocalizationService _localizationService;
        private readonly ILongLastingUserActionManager _longLastingUserActionManager;

        public ShowTimetableSelectingFacultyMenuAction(
            TimetableFacultyListKeyboardCreator keyboardCreator,
            IFacultiesProvider facultiesProvider,
            ITelegramBotService telegramBotService,
            ICurrentTelegramUserService currentUserService,
            ILocalizationService localizationService,
            ILongLastingUserActionManager longLastingUserActionManager)
        {
            _keyboardCreator = keyboardCreator;
            _facultiesProvider = facultiesProvider;
            _telegramBotService = telegramBotService;
            _currentUserService = currentUserService;
            _localizationService = localizationService;
            _longLastingUserActionManager = longLastingUserActionManager;
        }

        public async Task Execute(ShowTimetableSelectingFacultyMenuOptions menuOptions)
        {
            var faculties = await _facultiesProvider.GetFaculties().ConfigureAwait(false);

            await _longLastingUserActionManager.UpsertUserAction(
                    new LongLastingUserActionArguments
                    {
                        TelegramRozkladUser = _currentUserService.TelegramRozkladUser,
                        UserActionType = LongLastingUserActionType.TimetableSelectingFaculty
                    })
                .ConfigureAwait(false);

            var replyKeyboard =
                _keyboardCreator.CreateMarkup(new TimetableFacultyListKeyboardOptions {Faculties = faculties});

            await _telegramBotService.SendOrEditMessageAsync(
                _currentUserService.ChatId,
                _localizationService[_currentUserService.Language, "choose-faculty-message"],
                replyMarkup: replyKeyboard);
        }
    }
    
    public class ShowTimetableSelectingFacultyMenuOptions
    {
        
    }
}