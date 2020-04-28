using System.Threading.Tasks;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class AddGroupCallbackHandler : ISpecificCallbackQueryHandler
    {
        private readonly ITelegramBotActions _botActions;

        public AddGroupCallbackHandler(ITelegramBotActions botActions)
        {
            _botActions = botActions;
        }

        public Task Handle(CallbackQueryData callbackQueryData)
        {
            return _botActions.ShowTimetableSelectingFacultyMenu();
        }
    }
}