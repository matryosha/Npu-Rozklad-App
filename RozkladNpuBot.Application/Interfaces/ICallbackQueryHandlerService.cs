using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Application.Interfaces
{
    public interface ICallbackQueryHandlerService
    {
        Task Handle(CallbackQuery callbackQuery);
    }
}
