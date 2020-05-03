using System.Threading.Tasks;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class OpenRemoveGroupsMenuCallbackHandler : ISpecificCallbackQueryHandler
    {
        private readonly ITelegramBotActions _botActions;

        public OpenRemoveGroupsMenuCallbackHandler(ITelegramBotActions botActions)
        {
            _botActions = botActions;
        }
        public Task Handle(CallbackQueryData callbackQueryData)
        {
            return _botActions.ShowTimetableFacultyGroupsRemoveMenu();
        }
    }
}