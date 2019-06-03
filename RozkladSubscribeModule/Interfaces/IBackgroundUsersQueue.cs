using System.Threading;
using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface IBackgroundUsersQueue
    {
        void QueueNewUser(SubscribedUser user);
        void QueueUserToDelete(SubscribedUser user);
        Task<SubscribedUser> DequeueNewUserAsync(CancellationToken cancellationToken);
        Task<SubscribedUser> DequeueUserToDeleteAsync(CancellationToken cancellationToken);
    }
}