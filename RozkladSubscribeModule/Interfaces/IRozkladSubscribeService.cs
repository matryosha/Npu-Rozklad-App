using RozkladNpuAspNetCore.Entities;

namespace RozkladSubscribeModuleClient.Interfaces
{
    internal interface IRozkladSubscribeService
    {
        void SubscribeUser(RozkladUser user);
        void UnsubscribeUser(RozkladUser user);
    }
}