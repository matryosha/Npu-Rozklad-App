using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Infrastructure.Interfaces
{
    public interface IBotService
    {
        ITelegramBotClient Client { get; }
        Task SendErrorMessage(ChatId id);
    }
}
