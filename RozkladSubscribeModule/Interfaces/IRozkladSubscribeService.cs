using NpuTimetableParser;
using RozkladNpuAspNetCore.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface IRozkladSubscribeService
    {
        void SubscribeUser(RozkladUser user, Group group);
        void UnsubscribeUser(RozkladUser user, Group group);
    }
}