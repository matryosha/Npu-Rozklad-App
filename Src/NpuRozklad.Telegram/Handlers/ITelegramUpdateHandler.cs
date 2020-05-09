using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers
{
    public interface ITelegramUpdateHandler
    {
        Task Handle(Update update);
    }
}