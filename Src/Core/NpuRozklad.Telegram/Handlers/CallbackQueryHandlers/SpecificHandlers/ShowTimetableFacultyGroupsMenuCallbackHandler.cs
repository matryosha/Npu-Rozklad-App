using System.Threading.Tasks;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class ShowTimetableFacultyGroupsMenuCallbackHandler : ISpecificCallbackQueryHandler
    {
        private readonly ITelegramBotActions _botActions;

        public ShowTimetableFacultyGroupsMenuCallbackHandler(ITelegramBotActions botActions)
        {
            _botActions = botActions;
        }
        public Task Handle(CallbackQueryData callbackQueryData)
        {
            return _botActions.ShowTimetableFacultyGroupsMenu();
        }
    }
}