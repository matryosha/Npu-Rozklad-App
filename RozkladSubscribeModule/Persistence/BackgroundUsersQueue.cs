using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Persistence
{
    internal class BackgroundUsersQueue :
        IBackgroundUsersQueue
    {
        private readonly ConcurrentQueue<SubscribedUser> _usersToAddQueue;
        private readonly ConcurrentQueue<SubscribedUser> _usersToDeleteQueue;
        private readonly SemaphoreSlim _addSignal;
        private readonly SemaphoreSlim _deleteSignal;

        public BackgroundUsersQueue()
        {
            _usersToAddQueue = new ConcurrentQueue<SubscribedUser>();
            _usersToDeleteQueue = new ConcurrentQueue<SubscribedUser>();
            _addSignal = new SemaphoreSlim(0);
            _deleteSignal = new SemaphoreSlim(0);
        }

        public void QueueNewUser(SubscribedUser user)
        {
            _usersToAddQueue.Enqueue(user);
            _addSignal.Release();
        }

        public void QueueUserToDelete(SubscribedUser user)
        {
            _usersToDeleteQueue.Enqueue(user);
            _deleteSignal.Release();
        }

        public async Task<SubscribedUser> DequeueNewUserAsync(CancellationToken cancellationToken)
        {
            await _addSignal.WaitAsync(cancellationToken);
            _usersToAddQueue.TryDequeue(out var user);

            return user;
        }

        public async Task<SubscribedUser> DequeueUserToDeleteAsync(CancellationToken cancellationToken)
        {
            await _deleteSignal.WaitAsync(cancellationToken);
            _usersToDeleteQueue.TryDequeue(out var user);

            return user;
        }
    }
}