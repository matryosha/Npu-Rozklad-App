using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface IUserNotifyService<in TNotifyPayload>
        where TNotifyPayload: INotifyPayload
    {
        Task Notify(SubscribedUser subscribedUser, TNotifyPayload payload);
    }
}