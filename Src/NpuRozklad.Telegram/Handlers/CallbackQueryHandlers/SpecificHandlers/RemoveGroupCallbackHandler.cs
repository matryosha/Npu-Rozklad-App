using System.Threading.Tasks;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class RemoveGroupCallbackHandler : ISpecificCallbackQueryHandler
    {
        private readonly ITelegramBotActions _botActions;

        public RemoveGroupCallbackHandler(ITelegramBotActions botActions)
        {
            _botActions = botActions;
        }
        public Task Handle(CallbackQueryData callbackQueryData)
        {
            return _botActions.ShowTimetableFacultyGroupsRemoveMenu();
        }
    }
}