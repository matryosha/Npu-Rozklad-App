using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot;

namespace NpuRozklad.Telegram.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        public TelegramBotService(string botApiToken)
        {
            Client = new TelegramBotClient(botApiToken);
        }
        public ITelegramBotClient Client { get; }
    }
}