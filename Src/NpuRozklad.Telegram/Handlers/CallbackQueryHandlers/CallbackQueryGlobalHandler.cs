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
        private readonly ICurrentScopeServiceProvider _currentScopeServiceProvider;

        public CallbackQueryGlobalHandler(SpecificCallbackQueryHandlerProvider queryHandlerProvider,
            ICurrentScopeServiceProvider currentScopeServiceProvider)
        {
            _queryHandlerProvider = queryHandlerProvider;
            _currentScopeServiceProvider = currentScopeServiceProvider;
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
            callbackQueryData.CallbackQueryId = callbackQuery.Id;
            
            var handler = _queryHandlerProvider.GetHandler(callbackQueryData.CallbackQueryActionType);
            return handler.Handle(callbackQueryData);
        }

        private void StoreMessageId(CallbackQuery callbackQuery)
        {
            var scopeMessageIdProvider = _currentScopeServiceProvider.GetService<ICurrentScopeMessageIdProvider>();
            scopeMessageIdProvider.MessageId = callbackQuery.Message?.MessageId;
        }
    }
}