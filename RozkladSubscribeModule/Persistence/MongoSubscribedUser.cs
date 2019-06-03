using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Persistence
{
    class MongoSubscribedUser
    {
        public int Id { get; set; }
        public SubscribedUser SubscribedUser { get; set; }
    }
}
