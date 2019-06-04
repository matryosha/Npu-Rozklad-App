using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Persistence
{
    class MongoSubscribedUser
    {
        public string Id { get; set; }
        public SubscribedUser SubscribedUser { get; set; }
    }
}
