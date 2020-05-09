using System.Threading.Tasks;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class AddGroupCallbackHandler : SpecificHandlerBase
    {
        private readonly ITelegramBotActions _botActions;

        public AddGroupCallbackHandler(ITelegramBotService telegramBotService, ITelegramBotActions botActions) 
            : base(telegramBotService)
        {
            _botActions = botActions;
        }

        protected override Task HandleImplementation(CallbackQueryData callbackQueryData)
        {
            return _botActions.ShowTimetableSelectingFacultyMenu();
        }
    }
}