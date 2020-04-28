using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.MessageHandlers
{
    public class TelegramMessageGlobalHandler : ITelegramMessageHandler
    {
        private readonly CommandHandler _commandHandler;
        private readonly LongLastingUserActionGeneralHandler _lastingUserActionGeneralHandler;
        private readonly MessageTextHandler _textHandler;

        public TelegramMessageGlobalHandler(
            CommandHandler commandHandler,
            LongLastingUserActionGeneralHandler lastingUserActionGeneralHandler,
            MessageTextHandler textHandler)
        {
            _commandHandler = commandHandler;
            _lastingUserActionGeneralHandler = lastingUserActionGeneralHandler;
            _textHandler = textHandler;
        }
        
        public async Task Handle(Message message)
        {
            if (await _commandHandler.Handle(message)) return;
            if (await _lastingUserActionGeneralHandler.Handle(message)) return;
            if (await _textHandler.Handle(message)) return;
            
            throw new System.NotImplementedException();
        }
    }
}