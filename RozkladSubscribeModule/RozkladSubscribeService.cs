using NpuTimetableParser;
using RozkladNpuAspNetCore.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule
{
    public class RozkladSubscribeService : IRozkladSubscribeService
    {

        public RozkladSubscribeService()
        {
        }

        public void SubscribeUser(RozkladUser user, Group group)
        {
            throw new System.NotImplementedException();
        }

        public void UnsubscribeUser(RozkladUser user, Group group)
        {
            throw new System.NotImplementedException();
        }
    }
}