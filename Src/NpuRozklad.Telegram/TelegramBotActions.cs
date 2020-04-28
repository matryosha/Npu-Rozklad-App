using System.Threading.Tasks;
using NpuRozklad.Telegram.BotActions;
using NpuRozklad.Telegram.Interfaces;

namespace NpuRozklad.Telegram
{
    public class TelegramBotActions : ITelegramBotActions
    {
        private readonly IExternalServiceFactory _externalServiceFactory;

        public TelegramBotActions(IExternalServiceFactory externalServiceFactory)
        {
            _externalServiceFactory = externalServiceFactory;
        }

        public Task ShowTimetableSelectingFacultyGroupToAddMenu(
            ShowTimetableSelectingFacultyGroupToAddMenuOptions options)
        {
            return _externalServiceFactory.GetService<ShowTimetableSelectingFacultyGroupToAddMenuAction>()
                .Execute(options);
        }

        public Task ShowTimetableFacultyGroupViewMenu(ShowTimetableFacultyGroupViewMenuOptions options)
        {
            return _externalServiceFactory.GetService<ShowTimetableFacultyGroupViewMenuAction>().Execute(options);
        }

        public Task ShowTimetableFacultyGroupsMenu(ShowTimetableFacultyGroupsMenuOptions options = null)
        {
            return _externalServiceFactory.GetService<ShowTimetableFacultyGroupsMenuAction>().Execute(options);
        }

        public Task ShowTimetableSelectingFacultyMenu(ShowTimetableSelectingFacultyMenuOptions options = null)
        {
            return _externalServiceFactory.GetService<ShowTimetableSelectingFacultyMenuAction>().Execute(options);
        }

        public Task ShowMainMenu(ShowMainMenuOptions options = null)
        {
            return _externalServiceFactory.GetService<ShowMainMenuAction>().Execute(options);
        }

        public Task ResetCurrentUser(ResetCurrentUserOptions options = null)
        {
            return _externalServiceFactory.GetService<ResetCurrentUserAction>().Execute(options);
        }

        public Task ShowIncorrectInputMessage(ShowIncorrectInputMessageOptions options = null)
        {
            return _externalServiceFactory.GetService<ShowIncorrectInputMessageAction>().Execute(options);
        }

        public Task ShowFacultyGroupsForFacultyDoesNotExistMessage(
            ShowFacultyGroupsForFacultyDoesNotExistMessageOptions options = null)
        {
            return _externalServiceFactory.GetService<ShowFacultyGroupsForFacultyDoesNotExistMessageAction>()
                .Execute(options);
        }

        public Task ShowApplicationVersion()
        {
            return _externalServiceFactory.GetService<ShowApplicationVersionAction>().Execute();
        }
    }
}