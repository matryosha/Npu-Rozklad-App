using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.MessageHandlers
{
    /// <summary>
    /// Handling message text starting with /
    /// E.g.: /start, /reset, /version
    /// </summary>
    public class CommandHandler
    {
        private readonly ITelegramBotActions _telegramBotActions;

        public CommandHandler(ITelegramBotActions telegramBotActions)
        {
            _telegramBotActions = telegramBotActions;
        }

        public async Task<bool> Handle(Message message)
        {
            var isHandled = true;
            var text = message.Text;

            switch (text)
            {
                case "/start":
                    await _telegramBotActions.ShowMainMenu();
                    break;
                case "/reset":
                    await _telegramBotActions.ResetCurrentUser();
                    break;
                case "/version":
                    await _telegramBotActions.ShowApplicationVersion();
                    break;
                default:
                    isHandled = false;
                    break;
            }

            return isHandled;
        }
    }
}