using Telegram.Bot;

namespace NpuRozklad.Telegram.Services.Interfaces
{
    public interface ITelegramBotService
    {
        ITelegramBotClient Client { get; }
    }
}