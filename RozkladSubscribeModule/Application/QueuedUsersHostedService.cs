using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Application
{
    internal class QueuedUsersHostedService: IHostedService
    {
        private readonly ILogger<QueuedUsersHostedService> _logger;
        private readonly IBackgroundUsersQueue _usersQueue;
        private readonly ISubscribedUsersRepository _usersRepository;
        private readonly CancellationTokenSource _shutdown =
            new CancellationTokenSource();

        private Task _usersToAddBackgroundTask;
        private Task _usersToDeleteBackgroundTask;


        public QueuedUsersHostedService(
            ILogger<QueuedUsersHostedService> logger,
            IBackgroundUsersQueue usersQueue,
            ISubscribedUsersRepository usersRepository)
        {
            _logger = logger;
            _usersQueue = usersQueue;
            _usersRepository = usersRepository;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Users Hosted Service is starting.");

            _usersToAddBackgroundTask = Task.Run(BackgroundProcessingNewUser);
            _usersToDeleteBackgroundTask = Task.Run(BackgroundProcessingUserToDelete);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Users Hosted Service is stopping.");
            _shutdown.Cancel();
            return Task.WhenAll(_usersToDeleteBackgroundTask, _usersToAddBackgroundTask);
        }

        private async Task BackgroundProcessingNewUser()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var newUser = await _usersQueue.DequeueNewUserAsync(_shutdown.Token)
                    .ConfigureAwait(false);

                try
                {
                    await _usersRepository.AddUser(newUser)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"Error occurred when trying to add new user {nameof(newUser)}.");
                }
            }
        }

        private async Task BackgroundProcessingUserToDelete()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var userToDelete = await _usersQueue.DequeueUserToDeleteAsync(_shutdown.Token)
                    .ConfigureAwait(false);

                try
                {
                    await _usersRepository.DeleteUser(userToDelete)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError(e,
                        $"Error occurred when trying to delete user {nameof(userToDelete)}.");
                }
            }
        }
    }
}
