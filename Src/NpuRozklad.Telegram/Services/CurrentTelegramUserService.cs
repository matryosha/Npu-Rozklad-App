using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Services
{
    public class CurrentTelegramUserService : ICurrentTelegramUserService
    {
        public TelegramRozkladUser TelegramRozkladUser { get; set; }
        public string Language => TelegramRozkladUser.Language;
        public ChatId ChatId { get; set; }
    }
}