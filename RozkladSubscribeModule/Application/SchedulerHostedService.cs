using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Application
{
    internal class SchedulerHostedService<TCheckPayload, TNotifyPayload> : IHostedService
        where TCheckPayload: ICheckPayload
        where TNotifyPayload: INotifyPayload
    {
        private readonly ILogger<SchedulerHostedService<TCheckPayload, TNotifyPayload>> _logger;
        private readonly ISubscribedUsersRepository _subscribedUsersRepository;
        private readonly ICheckScheduleDiffService<TCheckPayload> _scheduleDiffService;
        private readonly IUserNotifyService<TNotifyPayload> _userNotifyService;
        private readonly ICheckToNotifyPayloadConverter<TCheckPayload, TNotifyPayload> _checkToNotifyPayloadConverter;
        private readonly TimeSpan _processPeriod;

        private Timer _timer;

        public SchedulerHostedService(
            ILogger<SchedulerHostedService<TCheckPayload, TNotifyPayload>> logger,
            ISubscribedUsersRepository subscribedUsersRepository,
            ICheckScheduleDiffService<TCheckPayload> scheduleDiffService,
            IUserNotifyService<TNotifyPayload> userNotifyService,
            ICheckToNotifyPayloadConverter<TCheckPayload, TNotifyPayload> checkToNotifyPayloadConverter,
            TimeSpan processPeriod = default
            )
        {
            _logger = logger;
            _subscribedUsersRepository = subscribedUsersRepository;
            _scheduleDiffService =  scheduleDiffService;
            _userNotifyService = userNotifyService;
            _checkToNotifyPayloadConverter = checkToNotifyPayloadConverter;

            if (processPeriod == default)
            {
                _processPeriod = TimeSpan.FromMinutes(10);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler Hosted Service is starting.");
            _timer = new Timer(ProcessSubscribedUsers, null, TimeSpan.Zero,
                _processPeriod);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Scheduler Hosted Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void ProcessSubscribedUsers(object state)
        {
            var subscribedUser = await _subscribedUsersRepository.GetUsersAsync()
                .ConfigureAwait(false);
            var distinctGroups = subscribedUser.GroupBy(
                u => u.FacultyShortName + '|' + u.GroupExternalId, u => u, (s, users) =>
                {
                    var user = users.FirstOrDefault();
                    return new
                    {
                        user.GroupExternalId,
                        user.FacultyShortName
                    };
                });


            foreach (var group in distinctGroups)
            {
                var checkPayload = _scheduleDiffService.CheckDiff(group.FacultyShortName, group.GroupExternalId);
                if (checkPayload.IsDiff())
                {
                    var currentGroupUsers = subscribedUser.Where(
                        u => u.GroupExternalId == group.GroupExternalId &&
                             u.FacultyShortName == group.FacultyShortName);

                    foreach (var user in currentGroupUsers)
                    {
                        _userNotifyService.Notify(
                            user,
                            _checkToNotifyPayloadConverter.Convert(
                                checkPayload));
                    }
                }
            }
        }
    }
}