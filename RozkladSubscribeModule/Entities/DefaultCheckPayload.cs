using RozkladSubscribeModuleClient.Interfaces;

namespace RozkladSubscribeModuleClient.Entities
{
    internal class DefaultCheckPayload : ICheckPayload
    {
        public bool IsDiff()
        {
            return false;
        }
    }
}