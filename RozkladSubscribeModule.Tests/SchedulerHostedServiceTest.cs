using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using RozkladSubscribeModuleClient.Application;
using RozkladSubscribeModuleClient.Entities;
using RozkladSubscribeModuleClient.Interfaces;
using Xunit;

namespace RozkladSubscribeModule.Tests
{
    public class SchedulerHostedServiceTest
    {
        [Fact]
        public async void Test1()
        {
            int notifyCount = 0;
            var notifyPayload = Mock.Of<INotifyPayload>();
            var checkPayload = Mock.Of<ICheckPayload>(payload => payload.IsDiff() == true);
            var logger = Mock.Of<ILogger<SchedulerHostedService<ICheckPayload, INotifyPayload>>>();
            var subscribedUsers = Mock.Of<ISubscribedUsersRepository>(sur => sur.GetUsers() == new List<SubscribedUser>
            {
                new SubscribedUser
                {
                    GroupExternalId = 1,
                    FacultyShortName = "first-faculty"
                },
                new SubscribedUser
                {
                    GroupExternalId = 1,
                    FacultyShortName = "first-faculty"
                }
            });
            var mockScheduleDiffService = new Mock<ICheckScheduleDiffService<ICheckPayload>>();
                mockScheduleDiffService.Setup(service => service.CheckDiff(It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(checkPayload);

            var mockUserNotifyService = new Mock<IUserNotifyService<INotifyPayload>>();
            mockUserNotifyService.Setup(
                    service => service.Notify(
                        It.IsAny<SubscribedUser>(), It.IsAny<INotifyPayload>()))
                .Callback(() => notifyCount++);


            var checkToNotifyPayloadConverter =
                new Mock<ICheckToNotifyPayloadConverter<ICheckPayload, INotifyPayload>>();
            checkToNotifyPayloadConverter.Setup(converter =>
                        converter.Convert(It.IsAny<ICheckPayload>())).Returns(notifyPayload);

            var schedulerHostedService = new SchedulerHostedService<ICheckPayload, INotifyPayload>(
                logger, subscribedUsers, mockScheduleDiffService.Object, mockUserNotifyService.Object,
                checkToNotifyPayloadConverter.Object, TimeSpan.FromSeconds(1000));

            await schedulerHostedService.StartAsync(CancellationToken.None);
            await Task.Delay(5000);
            Assert.Equal(2, notifyCount);
        }
    }
}
