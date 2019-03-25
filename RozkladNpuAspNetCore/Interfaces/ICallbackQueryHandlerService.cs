using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Interfaces
{
    public interface ICallbackQueryHandlerService
    {
        Task Handle(CallbackQuery callbackQuery);
    }
}
