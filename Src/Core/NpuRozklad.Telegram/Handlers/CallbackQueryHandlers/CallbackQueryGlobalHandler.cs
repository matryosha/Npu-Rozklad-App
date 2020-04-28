using System.Threading.Tasks;
using NpuRozklad.Telegram.Helpers;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers
{
    internal class CallbackQueryGlobalHandler : ICallbackQueryHandler
    {
        private readonly SpecificCallbackQueryHandlerProvider _queryHandlerProvider;

        public CallbackQueryGlobalHandler(SpecificCallbackQueryHandlerProvider queryHandlerProvider)
        {
            _queryHandlerProvider = queryHandlerProvider;
        }
        
        public Task Handle(CallbackQuery callbackQuery)
        {
            /* 
             * Callback data to KeyValue pairs
             *               \/
             *               V
             * Get certain handler depending on CallbackQueryActionType
             * Delegate handling to specific handler
             * 
             */

            var callbackQueryData = CallbackDataFormatter.DeserializeCallbackQueryData(callbackQuery.Data);
            var handler = _queryHandlerProvider.GetHandler(callbackQueryData.CallbackQueryActionType);
            return handler.Handle(callbackQueryData);
        }
    }
}