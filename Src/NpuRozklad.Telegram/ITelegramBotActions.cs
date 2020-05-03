using System.Threading.Tasks;
using NpuRozklad.Telegram.BotActions;

namespace NpuRozklad.Telegram
{
    public interface ITelegramBotActions
    {
        Task ShowTimetableSelectingFacultyGroupToAddMenu(ShowTimetableSelectingFacultyGroupToAddMenuOptions options);
        Task ShowTimetableFacultyGroupViewMenu(ShowTimetableFacultyGroupViewMenuOptions options);
        Task ShowTimetableFacultyGroupsMenu(ShowTimetableFacultyGroupsMenuOptions options = null);
        Task ShowTimetableSelectingFacultyMenu(ShowTimetableSelectingFacultyMenuOptions options = null);
        Task ShowMainMenu(ShowMainMenuOptions options = null);
        Task ResetCurrentUser(ResetCurrentUserOptions options = null);
        Task ShowIncorrectInputMessage(ShowIncorrectInputMessageOptions options = null);
        Task ShowFacultyGroupsForFacultyDoesNotExistMessage(
            ShowFacultyGroupsForFacultyDoesNotExistMessageOptions options = null);
        Task ShowApplicationVersion();
        Task ShowTimetableFacultyGroupsRemoveMenu();
    }
}