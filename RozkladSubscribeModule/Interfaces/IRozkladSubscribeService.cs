using NpuTimetableParser;
using RozkladNpuBot.Domain.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    public interface IRozkladSubscribeService
    {
        void SubscribeUser(RozkladUser user, long chatId, Group group);
        void UnsubscribeUser(RozkladUser user, long chatId, Group group);
    }
}