using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Services.Interfaces
{
    public interface ICurrentTelegramUserService
    {
        TelegramRozkladUser TelegramRozkladUser { get; set; }
        string Language { get; }
        ChatId ChatId { get; set; }
    }
}