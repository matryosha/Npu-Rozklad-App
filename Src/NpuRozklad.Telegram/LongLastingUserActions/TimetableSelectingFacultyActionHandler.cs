using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TimetableSelectingFacultyActionHandler> _logger;

        public TimetableSelectingFacultyActionHandler(
            ITelegramBotActions botActions,
            IFacultiesProvider facultiesProvider,
            IFacultyGroupsProvider facultyGroupsProvider,
            ILongLastingUserActionManager longLastingUserActionManager,
            ICurrentUserLocalizationService currentUserLocalizationService,
            ILogger<TimetableSelectingFacultyActionHandler> logger)
        {
            _botActions = botActions;
            _facultiesProvider = facultiesProvider;
            _facultyGroupsProvider = facultyGroupsProvider;
            _longLastingUserActionManager = longLastingUserActionManager;
            _currentUserLocalizationService = currentUserLocalizationService;
            _logger = logger;
        }

        public async Task<bool> Handle(LongLastingUserActionArguments userActionArguments)
        {
            string userInput = null;
            ICollection<Faculty> faculties = null;
            Faculty selectedFaculty = null;
            ICollection<Group> facultyGroups = null;
            
            try
            {
                var isHandled = true;

                userInput = (userActionArguments.Parameters[typeof(Message)] as Message)?.Text;

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

                faculties = await _facultiesProvider.GetFaculties();
                selectedFaculty = faculties.FirstOrDefault(f => f.FullName == userInput);

                if (selectedFaculty == null)
                {
                    await _botActions.ShowMessage(o =>
                        o.MessageTextLocalizationValue = "such-faculty-was-not-found");
                    return isHandled;
                }

                facultyGroups = await _facultyGroupsProvider.GetFacultyGroups(selectedFaculty);

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
            catch (Exception e)
            {

                var facultiesString = string.Empty;
                
                if (faculties != null)
                    facultiesString = string.Join(",", faculties?.Select(f => f.ToString()));

                var facultyGroupsString = string.Empty;

                if (facultyGroups != null)
                    facultyGroupsString = string.Join(",", facultyGroups.Select(f => f.ToString()));
                
                _logger.LogError(TelegramLogEvents.TimetableSelectingFacultyError, e,
                    "userInput: {userInput}. " +
                    "faculties: {facultiesString}. " +
                    "selectedFaculty: {selectedFaculty}. " +
                    "facultyGroups: {facultyGroupsString}. " +
                    "userActionArguments: {userActionArguments}. ",
                    userInput, facultiesString, selectedFaculty, facultyGroupsString, userActionArguments);
                
                throw;
            }
        }
    }
}