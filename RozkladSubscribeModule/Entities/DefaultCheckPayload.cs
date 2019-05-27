using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Entities
{
    internal class DefaultCheckPayload : ICheckPayload
    {
        public bool IsDiff()
        {
            return false;
        }
    }
}