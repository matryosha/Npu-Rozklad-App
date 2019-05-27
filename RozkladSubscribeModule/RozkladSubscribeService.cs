using System.Threading.Tasks;
using RozkladNpuAspNetCore.Entities;
using RozkladSubscribeModuleClient.Interfaces;

namespace RozkladSubscribeModuleClient
{
    public class RozkladSubscribeService : IRozkladSubscribeService
    {

        public RozkladSubscribeService()
        {
        }


        public void SubscribeUser(RozkladUser user)
        {
            throw new System.NotImplementedException();
        }

        public void UnsubscribeUser(RozkladUser user)
        {
            throw new System.NotImplementedException();
        }
    }
}