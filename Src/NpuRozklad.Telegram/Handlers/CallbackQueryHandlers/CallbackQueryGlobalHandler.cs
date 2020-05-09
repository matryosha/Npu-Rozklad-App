using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers;
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
        private readonly ILogger<CallbackQueryGlobalHandler> _logger;

        public CallbackQueryGlobalHandler(SpecificCallbackQueryHandlerProvider queryHandlerProvider,
            ICurrentScopeServiceProvider currentScopeServiceProvider,
            ILogger<CallbackQueryGlobalHandler> logger)
        {
            _queryHandlerProvider = queryHandlerProvider;
            _currentScopeServiceProvider = currentScopeServiceProvider;
            _logger = logger;
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
            CallbackQueryData callbackQueryData = null;
            ISpecificCallbackQueryHandler handler = null;
            
            try
            {
                StoreMessageId(callbackQuery);
                callbackQueryData = CallbackDataFormatter.DeserializeCallbackQueryData(callbackQuery.Data);
                callbackQueryData.CallbackQueryId = callbackQuery.Id;
            
                handler = _queryHandlerProvider.GetHandler(callbackQueryData.CallbackQueryActionType);
                return handler.Handle(callbackQueryData);
            }
            catch (Exception e)
            {
                var callbackQueryId = callbackQuery.Id;
                var user = callbackQuery.From;

                var messageId = callbackQuery.Message?.MessageId;
                var messageText = callbackQuery.Message?.Text;
                
                _logger.LogError(TelegramLogEvents.CallbackQueryHandlerError, e,
                    "Telegram callbackQueryId: {callbackQueryId}. " +
                    "Telegram user: {user}. " +
                    "Callback query message id: {messageId}. " +
                    "Message text: {messageText}" +
                    "callbackQueryData: {callbackQueryData}. " +
                    "handler: {handler}. ",
                    callbackQueryId, user, messageId, messageText, callbackQueryData, handler);
                
                throw;
            }
        }

        private void StoreMessageId(CallbackQuery callbackQuery)
        {
            var scopeMessageIdProvider = _currentScopeServiceProvider.GetService<ICurrentScopeMessageIdProvider>();
            scopeMessageIdProvider.MessageId = callbackQuery.Message?.MessageId;
        }
    }
}