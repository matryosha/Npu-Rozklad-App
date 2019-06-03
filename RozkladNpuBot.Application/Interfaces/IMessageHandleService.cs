using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Application.Interfaces
{
    public interface IMessageHandleService
    {
        Task HandleMessage(Message message);
    }
}
