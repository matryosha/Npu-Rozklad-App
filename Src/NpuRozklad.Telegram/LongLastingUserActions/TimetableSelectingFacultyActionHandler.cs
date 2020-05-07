using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.BotActions;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class TimetableSelectingFacultyActionHandler : ILongLastingUserActionHandler
    {
        private readonly ITelegramBotActions _botActions;
        private readonly IFacultiesProvider _facultiesProvider;
        private readonly IFacultyGroupsProvider _facultyGroupsProvider;
        private readonly ILongLastingUserActionManager _longLastingUserActionManager;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public TimetableSelectingFacultyActionHandler(
            ITelegramBotActions botActions,
            IFacultiesProvider facultiesProvider,
            IFacultyGroupsProvider facultyGroupsProvider,
            ILongLastingUserActionManager longLastingUserActionManager,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _botActions = botActions;
            _facultiesProvider = facultiesProvider;
            _facultyGroupsProvider = facultyGroupsProvider;
            _longLastingUserActionManager = longLastingUserActionManager;
            _currentUserLocalizationService = currentUserLocalizationService;
        }

        public async Task<bool> Handle(LongLastingUserActionArguments userActionArguments)
        {
            var isHandled = true;

            var userInput = (userActionArguments.Parameters[typeof(Message)] as Message)?.Text;

            if (string.IsNullOrWhiteSpace(userInput))
            {
                await _botActions.ShowMessage(o => o.ShowIncorrectInputMessage = true);
                return isHandled;
            }

            if (userInput == _currentUserLocalizationService["back"])
            {
                await _longLastingUserActionManager.ClearUserAction(userActionArguments.TelegramRozkladUser);
                await _botActions.ShowMainMenu();
                return true;
            }

            var faculties = await _facultiesProvider.GetFaculties();
            var selectedFaculty = faculties.FirstOrDefault(f => f.FullName == userInput);

            if (selectedFaculty == null)
            {
                await _botActions.ShowMessage(o =>
                    o.MessageTextLocalizationValue = "such-faculty-was-not-found");
                return isHandled;
            }

            var facultyGroups = await _facultyGroupsProvider.GetFacultyGroups(selectedFaculty);

            if (!facultyGroups.Any())
            {
                await _longLastingUserActionManager.ClearUserAction(userActionArguments.TelegramRozkladUser);
                await _botActions.ShowFacultyGroupsForFacultyDoesNotExistMessage();
                return isHandled;
            }

            await _longLastingUserActionManager.UpsertUserAction(new LongLastingUserActionArguments
            {
                TelegramRozkladUser = userActionArguments.TelegramRozkladUser,
                UserActionType = LongLastingUserActionType.TimetableSelectingFacultyGroupToAdd,
                Parameters = new Dictionary<Type, object>
                {
                    {typeof(ICollection<Group>), facultyGroups},
                    {typeof(Faculty), selectedFaculty}
                }
            });
            
            await _botActions.ShowTimetableSelectingFacultyGroupToAddMenu(
                new ShowTimetableSelectingFacultyGroupToAddMenuOptions
                {
                    FacultyGroups = facultyGroups
                });

            return isHandled;
        }
    }
}