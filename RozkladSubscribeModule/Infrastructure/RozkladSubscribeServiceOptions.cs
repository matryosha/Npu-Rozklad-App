using System;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Infrastructure
{
    public class RozkladSubscribeServiceOptions
    {
        public string SubscribedUsersDbConnectionString { get; set; }
        public CheckTimeType CheckTimeType { get; set; }
        public TimeSpan ProcessPeriod { get; set; } = TimeSpan.FromMinutes(10);
    }
}
