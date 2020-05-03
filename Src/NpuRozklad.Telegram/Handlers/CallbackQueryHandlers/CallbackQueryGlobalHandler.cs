using System.Threading.Tasks;
using NpuRozklad.Telegram.Helpers;
using NpuRozklad.Telegram.Interfaces;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers
{
    internal class CallbackQueryGlobalHandler : ICallbackQueryHandler
    {
        private readonly SpecificCallbackQueryHandlerProvider _queryHandlerProvider;
        private readonly IExternalServiceProvider _externalServiceProvider;

        public CallbackQueryGlobalHandler(SpecificCallbackQueryHandlerProvider queryHandlerProvider,
            IExternalServiceProvider externalServiceProvider)
        {
            _queryHandlerProvider = queryHandlerProvider;
            _externalServiceProvider = externalServiceProvider;
        }
        
        public Task Handle(CallbackQuery callbackQuery)
        {
            /*
             * Get message id from callback and store it
             * 
             * Callback data to KeyValue pairs
             *               \/
             *               V
             * Get certain handler depending on CallbackQueryActionType
             * Delegate handling to specific handler
             * 
             */
            StoreMessageId(callbackQuery);
            var callbackQueryData = CallbackDataFormatter.DeserializeCallbackQueryData(callbackQuery.Data);
            var handler = _queryHandlerProvider.GetHandler(callbackQueryData.CallbackQueryActionType);
            return handler.Handle(callbackQueryData);
        }

        private void StoreMessageId(CallbackQuery callbackQuery)
        {
            var scopeMessageIdProvider = _externalServiceProvider.GetService<ICurrentScopeMessageIdProvider>();
            scopeMessageIdProvider.MessageId = callbackQuery.Message?.MessageId;
        }
    }
}