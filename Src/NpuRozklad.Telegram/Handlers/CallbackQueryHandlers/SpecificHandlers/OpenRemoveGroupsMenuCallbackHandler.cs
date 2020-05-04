using System.Threading.Tasks;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class OpenRemoveGroupsMenuCallbackHandler : SpecificHandlerBase
    {
        private readonly ITelegramBotActions _botActions;

        public OpenRemoveGroupsMenuCallbackHandler(ITelegramBotActions botActions, ITelegramBotService telegramBotService)
         : base(telegramBotService)
        {
            _botActions = botActions;
        }
        
        protected override Task HandleImplementation(CallbackQueryData callbackQueryData)
        {
            return _botActions.ShowTimetableFacultyGroupsRemoveMenu();
        }
    }
}