using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Services.Interfaces
{
    public interface ITelegramUserThrottle
    {
        bool ShouldSkipProcessing(Update update);
    }
}