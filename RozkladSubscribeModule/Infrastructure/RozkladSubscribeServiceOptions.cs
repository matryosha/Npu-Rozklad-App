using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure.Enums;

namespace RozkladSubscribeModule.Infrastructure
{
    public class RozkladSubscribeServiceOptions
    {
        public string SubscribedUsersDbConnectionString { get; set; }
        public CheckTimeType CheckTimeType { get; set; }
    }
}
