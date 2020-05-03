using System.Threading.Tasks;
using NpuRozklad.Telegram.BotActions;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram
{
    public class TelegramBotActions : ITelegramBotActions
    {
        private readonly IExternalServiceProvider _externalServiceProvider;

        public TelegramBotActions(IExternalServiceProvider externalServiceProvider)
        {
            _externalServiceProvider = externalServiceProvider;
        }

        public Task ShowTimetableSelectingFacultyGroupToAddMenu(
            ShowTimetableSelectingFacultyGroupToAddMenuOptions options)
        {
            return _externalServiceProvider.GetService<ShowTimetableSelectingFacultyGroupToAddMenuAction>()
                .Execute(options);
        }

        public Task ShowTimetableFacultyGroupViewMenu(ShowTimetableFacultyGroupViewMenuOptions options)
        {
            return _externalServiceProvider.GetService<ShowTimetableFacultyGroupViewMenuAction>().Execute(options);
        }

        public Task ShowTimetableFacultyGroupsMenu(ShowTimetableFacultyGroupsMenuOptions options = null)
        {
            return _externalServiceProvider.GetService<ShowTimetableFacultyGroupsMenuAction>().Execute(options);
        }

        public Task ShowTimetableSelectingFacultyMenu(ShowTimetableSelectingFacultyMenuOptions options = null)
        {
            return _externalServiceProvider.GetService<ShowTimetableSelectingFacultyMenuAction>().Execute(options);
        }

        public Task ShowMainMenu(ShowMainMenuOptions options = null)
        {
            return _externalServiceProvider.GetService<ShowMainMenuAction>().Execute(options);
        }

        public Task ResetCurrentUser(ResetCurrentUserOptions options = null)
        {
            return _externalServiceProvider.GetService<ResetCurrentUserAction>().Execute(options);
        }

        public Task ShowIncorrectInputMessage(ShowIncorrectInputMessageOptions options = null)
        {
            return _externalServiceProvider.GetService<ShowIncorrectInputMessageAction>().Execute(options);
        }

        public Task ShowFacultyGroupsForFacultyDoesNotExistMessage(
            ShowFacultyGroupsForFacultyDoesNotExistMessageOptions options = null)
        {
            return _externalServiceProvider.GetService<ShowFacultyGroupsForFacultyDoesNotExistMessageAction>()
                .Execute(options);
        }

        public Task ShowApplicationVersion()
        {
            return _externalServiceProvider.GetService<ShowApplicationVersionAction>().Execute();
        }

        public Task ShowTimetableFacultyGroupsRemoveMenu()
        {
            return _externalServiceProvider.GetService<ShowTimetableFacultyGroupsRemoveMenuAction>().Execute();
        }
    }
}