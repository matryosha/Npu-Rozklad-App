using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface IUserNotifyService<in TNotifyPayload>
        where TNotifyPayload: INotifyPayload
    {
        void Notify(SubscribedUser subscribedUser, TNotifyPayload payload);
    }
}