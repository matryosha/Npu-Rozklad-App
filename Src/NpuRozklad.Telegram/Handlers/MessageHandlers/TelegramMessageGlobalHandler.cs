using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.MessageHandlers
{
    public class TelegramMessageGlobalHandler : ITelegramMessageHandler
    {
        private readonly CommandHandler _commandHandler;
        private readonly LongLastingUserActionGeneralHandler _lastingUserActionGeneralHandler;
        private readonly MessageTextHandler _textHandler;
        private readonly ILogger<TelegramMessageGlobalHandler> _logger;

        public TelegramMessageGlobalHandler(
            CommandHandler commandHandler,
            LongLastingUserActionGeneralHandler lastingUserActionGeneralHandler,
            MessageTextHandler textHandler,
            ILogger<TelegramMessageGlobalHandler> logger)
        {
            _commandHandler = commandHandler;
            _lastingUserActionGeneralHandler = lastingUserActionGeneralHandler;
            _textHandler = textHandler;
            _logger = logger;
        }
        
        public async Task Handle(Message message)
        {
            try
            {
                if (await _commandHandler.Handle(message)) return;
                if (await _lastingUserActionGeneralHandler.Handle(message)) return;
                if (await _textHandler.Handle(message)) return;
            
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                var messageId = message.MessageId;
                var messageFromUser = message.From;
                var messageText = message.Text;
                
                _logger.LogError(TelegramLogEvents.TelegramMessageHandlerError, e, 
                    "Telegram message id: {messageId}. " +
                    "Message from user: {messageFromUser}. " +
                    "Message text: {messageText}. ", 
                    messageId, messageFromUser, messageText);
                
                throw;
            }
        }
    }
}