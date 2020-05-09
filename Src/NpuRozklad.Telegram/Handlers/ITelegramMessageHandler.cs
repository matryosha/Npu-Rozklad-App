using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers
{
    public interface ITelegramMessageHandler
    {
        Task Handle(Message message);
    }
}