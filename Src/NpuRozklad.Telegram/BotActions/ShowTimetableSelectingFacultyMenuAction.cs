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
        private readonly ICurrentTelegramUserContext _currentUserContext;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;
        private readonly ILongLastingUserActionManager _longLastingUserActionManager;

        public ShowTimetableSelectingFacultyMenuAction(
            TimetableFacultyListKeyboardCreator keyboardCreator,
            IFacultiesProvider facultiesProvider,
            ITelegramBotService telegramBotService,
            ICurrentTelegramUserContext currentUserContext,
            ICurrentUserLocalizationService currentUserLocalizationService,
            ILongLastingUserActionManager longLastingUserActionManager)
        {
            _keyboardCreator = keyboardCreator;
            _facultiesProvider = facultiesProvider;
            _telegramBotService = telegramBotService;
            _currentUserContext = currentUserContext;
            _currentUserLocalizationService = currentUserLocalizationService;
            _longLastingUserActionManager = longLastingUserActionManager;
        }

        public async Task Execute(ShowTimetableSelectingFacultyMenuOptions menuOptions)
        {
            var faculties = await _facultiesProvider.GetFaculties().ConfigureAwait(false);

            await _longLastingUserActionManager.UpsertUserAction(
                    new LongLastingUserActionArguments
                    {
                        TelegramRozkladUser = _currentUserContext.TelegramRozkladUser,
                        UserActionType = LongLastingUserActionType.TimetableSelectingFaculty
                    })
                .ConfigureAwait(false);

            var replyKeyboard =
                _keyboardCreator.CreateMarkup(new TimetableFacultyListKeyboardOptions {Faculties = faculties});

            await _telegramBotService.SendOrEditMessageAsync(
                _currentUserLocalizationService["choose-faculty-message"],
                replyMarkup: replyKeyboard);
        }
    }
    
    public class ShowTimetableSelectingFacultyMenuOptions
    {
        
    }
}