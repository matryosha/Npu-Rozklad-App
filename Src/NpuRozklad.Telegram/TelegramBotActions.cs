using System;
using System.Threading.Tasks;
using NpuRozklad.Telegram.BotActions;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram
{
    public class TelegramBotActions : ITelegramBotActions
    {
        private readonly ICurrentScopeServiceProvider _currentScopeServiceProvider;

        public TelegramBotActions(ICurrentScopeServiceProvider currentScopeServiceProvider)
        {
            _currentScopeServiceProvider = currentScopeServiceProvider;
        }

        public Task ShowTimetableSelectingFacultyGroupToAddMenu(
            ShowTimetableSelectingFacultyGroupToAddMenuOptions options)
        {
            return _currentScopeServiceProvider.GetService<ShowTimetableSelectingFacultyGroupToAddMenuAction>()
                .Execute(options);
        }

        public Task ShowTimetableFacultyGroupViewMenu(ShowTimetableFacultyGroupViewMenuOptions options)
        {
            return _currentScopeServiceProvider.GetService<ShowTimetableFacultyGroupViewMenuAction>().Execute(options);
        }

        public Task ShowTimetableFacultyGroupsMenu(ShowTimetableFacultyGroupsMenuOptions options = null)
        {
            return _currentScopeServiceProvider.GetService<ShowTimetableFacultyGroupsMenuAction>().Execute(options);
        }

        public Task ShowTimetableSelectingFacultyMenu(ShowTimetableSelectingFacultyMenuOptions options = null)
        {
            return _currentScopeServiceProvider.GetService<ShowTimetableSelectingFacultyMenuAction>().Execute(options);
        }

        public Task ShowMainMenu(ShowMainMenuOptions options = null)
        {
            return _currentScopeServiceProvider.GetService<ShowMainMenuAction>().Execute(options);
        }

        public Task ResetCurrentUser(ResetCurrentUserOptions options = null)
        {
            return _currentScopeServiceProvider.GetService<ResetCurrentUserAction>().Execute(options);
        }

        public Task ShowMessage(Action<ShowMessageOptions> options)
        {
            return _currentScopeServiceProvider.GetService<ShowMessageAction>().Execute(options);
        }

        public Task ShowFacultyGroupsForFacultyDoesNotExistMessage(
            ShowFacultyGroupsForFacultyDoesNotExistMessageOptions options = null)
        {
            return _currentScopeServiceProvider.GetService<ShowFacultyGroupsForFacultyDoesNotExistMessageAction>()
                .Execute(options);
        }

        public Task ShowApplicationVersion()
        {
            return _currentScopeServiceProvider.GetService<ShowApplicationVersionAction>().Execute();
        }

        public Task ShowTimetableFacultyGroupsRemoveMenu()
        {
            return _currentScopeServiceProvider.GetService<ShowTimetableFacultyGroupsRemoveMenuAction>().Execute();
        }
    }
}