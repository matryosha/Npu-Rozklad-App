using System.Threading.Tasks;
using NpuTimetableParser;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Application.Interfaces
{
    public interface IKeyboardReplyService
    {
        Task<Message> ShowMainMenu(Message message);
        Task<Message> ShowFacultyList(Message message);
        Task<Message> ShowGroupList(Message message, Faculty faculty);
    }
}
