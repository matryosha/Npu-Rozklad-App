using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers
{
    public interface ICallbackQueryHandler
    {
        Task Handle(CallbackQuery callbackQuery);
    }
}