using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.BotActions;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class TimetableFacultyGroupsMenuGroupSelectedHandler : ISpecificCallbackQueryHandler
    {
        private readonly ITelegramBotActions _telegramBotActions;
        private readonly IFacultiesProvider _facultiesProvider;
        private readonly IFacultyGroupsProvider _facultyGroupsProvider;
        private readonly ILocalDateService _localDateService;

        private CallbackQueryData _callbackQueryData;
        private string _facultyTypeId;
        private string _facultyGroupTypeId;
        private Faculty _faculty;
        private Group _facultyGroup;
        
        public TimetableFacultyGroupsMenuGroupSelectedHandler(
            ITelegramBotActions telegramBotActions,
            IFacultiesProvider facultiesProvider,
            IFacultyGroupsProvider facultyGroupsProvider,
            ILocalDateService localDateService)
        {
            _telegramBotActions = telegramBotActions;
            _facultiesProvider = facultiesProvider;
            _facultyGroupsProvider = facultyGroupsProvider;
            _localDateService = localDateService;
        }

        public async Task Handle(CallbackQueryData callbackQueryData)
        {
            _callbackQueryData = callbackQueryData;
            ProcessCallbackQueryData();

            await GetFaculty();
            await GetFacultyGroup();

            var currentDayOfWeek = _localDateService.LocalDateTime.DayOfWeek;

            var actionOptions = new ShowTimetableFacultyGroupViewMenuOptions
            {
                FacultyGroup = _facultyGroup,
                DayOfWeek = currentDayOfWeek,
                IsNextWeekSelected = false
            };

            await _telegramBotActions.ShowTimetableFacultyGroupViewMenu(actionOptions);
        }
        
        private async Task GetFacultyGroup()
        {
            var facultyGroupTypeId = _facultyGroupTypeId;
            var facultyGroups = await _facultyGroupsProvider.GetFacultyGroups(_faculty);
            _facultyGroup = facultyGroups.FirstOrDefault(g => g.TypeId == facultyGroupTypeId);
        }
        private async Task GetFaculty()
        {
            var faculties = await _facultiesProvider.GetFaculties();
            var facultyTypeId = _facultyTypeId;
            _faculty = faculties.FirstOrDefault(f => f.TypeId == facultyTypeId);
        }
        private void ProcessCallbackQueryData()
        {
            var values = _callbackQueryData.Values;

            _facultyGroupTypeId = values[0];
            _facultyTypeId = values[1];
        }
    }
}