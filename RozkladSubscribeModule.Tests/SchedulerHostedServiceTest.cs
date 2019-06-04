using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using RozkladSubscribeModule.Application;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure;
using RozkladSubscribeModule.Interfaces;
using Xunit;

namespace RozkladSubscribeModule.Tests
{
    public class SchedulerHostedServiceTest : SchedulerHostedServiceTestBase
    {
        [Fact]
        public async void NotifyAll_DiffGroupDiffFaculty()
        {
            int notifyCount = 0;
            var users = GetDiffGroupDiffFacultySubscribedUsers();


            await GetDefaultHostedServiceTestSubscribedUser(users, () => notifyCount++)
                .StartAsync(CancellationToken.None);
            await Task.Delay(1000);

            Assert.Equal(35, notifyCount);
        }

        [Fact]
        public async void NotifyAll_DiffGroupSameFaculty() {
            int notifyCount = 0;
            var users = new List<SubscribedUser>();
            users.AddRange(Helpers.CreateUsers(5, 1, "first"));
            users.AddRange(Helpers.CreateUsers(5, 2, "first"));
            users.AddRange(Helpers.CreateUsers(5, 3, "first"));
            users.AddRange(Helpers.CreateUsers(5, 4, "first"));
            users.AddRange(Helpers.CreateUsers(5, 5, "first"));
            users.AddRange(Helpers.CreateUsers(5, 6, "first"));
            users.AddRange(Helpers.CreateUsers(5, 7, "first"));


            await GetDefaultHostedServiceTestSubscribedUser(users, () => notifyCount++)
                .StartAsync(CancellationToken.None);
            await Task.Delay(1500);

            Assert.Equal(35, notifyCount);
        }

        [Fact]
        public async void NotifyOnlyFirstGroup_DiffGroupSameFaculty() {
            int notifyCount = 0;
            var users = GetDiffGroupSameFacultySubscribedUsers();

            var subscribedUsersRepo = GetMockSubscribedUsersRepositoryWithUsers(users);
            var mockScheduleDiffService = GetMockScheduleDiffServiceOnlyFirstReturnsTrue();

            var mockUserNotifyService = GetMockIUserNotifyService;
            mockUserNotifyService.Setup(s =>
                s.Notify(
                    It.Is<SubscribedUser>(user => user.GroupExternalId == 1), 
                    It.IsAny<INotifyPayload>()))
                .Callback(() => notifyCount++);

            var mockCheckToNotifyPayloadConverter =
                GetCheckToNotifyPayloadConverterConvertAnyReturns(_emptyNotifyPayload);

            var scheduler = new SchedulerHostedService<ICheckPayload, INotifyPayload>(
                _emptyLogger, subscribedUsersRepo, mockScheduleDiffService.Object, mockUserNotifyService.Object,
                mockCheckToNotifyPayloadConverter, GetOptions());

            await scheduler.StartAsync(CancellationToken.None);
            await Task.Delay(1000);

            Assert.Equal(5, notifyCount);
        }

        [Fact]
        public async void NotifyOnlySecondGroup_DiffGroupSameFaculty() {
            int notifyCount = 0;
            var users = GetDiffGroupSameFacultySubscribedUsers();

            var subscribedUsersRepo = GetMockSubscribedUsersRepositoryWithUsers(users);
            var mockScheduleDiffService = GetMockScheduleDiffServiceOnlySecondReturnsTrue();

            var mockUserNotifyService = GetMockIUserNotifyService;
            mockUserNotifyService.Setup(s =>
                    s.Notify(
                        It.Is<SubscribedUser>(user => user.GroupExternalId == 2),
                        It.IsAny<INotifyPayload>()))
                .Callback(() => notifyCount++);

            var mockCheckToNotifyPayloadConverter =
                GetCheckToNotifyPayloadConverterConvertAnyReturns(_emptyNotifyPayload);

            var scheduler = new SchedulerHostedService<ICheckPayload, INotifyPayload>(
                _emptyLogger, subscribedUsersRepo, mockScheduleDiffService.Object, mockUserNotifyService.Object,
                mockCheckToNotifyPayloadConverter, GetOptions());

            await scheduler.StartAsync(CancellationToken.None);
            await Task.Delay(1000);

            Assert.Equal(5, notifyCount);
        }

        [Fact]
        public async void NotifyOnlyFirstGroup_DiffGroupDiffFaculty() {
            int notifyCount = 0;
            var users = GetDiffGroupDiffFacultySubscribedUsers();

            var subscribedUsersRepo = GetMockSubscribedUsersRepositoryWithUsers(users);
            var mockScheduleDiffService = GetMockScheduleDiffServiceOnlyFirstReturnsTrue();

            var mockUserNotifyService = GetMockIUserNotifyService;
            mockUserNotifyService.Setup(s =>
                    s.Notify(
                        It.Is<SubscribedUser>(user => user.GroupExternalId == 1),
                        It.IsAny<INotifyPayload>()))
                .Callback(() => notifyCount++);

            var mockCheckToNotifyPayloadConverter =
                GetCheckToNotifyPayloadConverterConvertAnyReturns(_emptyNotifyPayload);

            var scheduler = new SchedulerHostedService<ICheckPayload, INotifyPayload>(
                _emptyLogger, subscribedUsersRepo, mockScheduleDiffService.Object, mockUserNotifyService.Object,
                mockCheckToNotifyPayloadConverter, GetOptions());

            await scheduler.StartAsync(CancellationToken.None);
            await Task.Delay(1000);

            Assert.Equal(5, notifyCount);
        }

        [Fact]
        public async void NotifyOnlySecondGroup_DiffGroupDiffFaculty() {
            int notifyCount = 0;
            var users = GetDiffGroupDiffFacultySubscribedUsers();

            var subscribedUsersRepo = GetMockSubscribedUsersRepositoryWithUsers(users);
            var mockScheduleDiffService = GetMockScheduleDiffServiceOnlySecondReturnsTrue();

            var mockUserNotifyService = GetMockIUserNotifyService;
            mockUserNotifyService.Setup(s =>
                    s.Notify(
                        It.Is<SubscribedUser>(user => user.GroupExternalId == 2),
                        It.IsAny<INotifyPayload>()))
                .Callback(() => notifyCount++);

            var mockCheckToNotifyPayloadConverter =
                GetCheckToNotifyPayloadConverterConvertAnyReturns(_emptyNotifyPayload);

            var scheduler = new SchedulerHostedService<ICheckPayload, INotifyPayload>(
                _emptyLogger, subscribedUsersRepo, mockScheduleDiffService.Object, mockUserNotifyService.Object,
                mockCheckToNotifyPayloadConverter, GetOptions());

            await scheduler.StartAsync(CancellationToken.None);
            await Task.Delay(1000);

            Assert.Equal(5, notifyCount);
        }

        [Fact]
        public async void NotifyOnlyFirstGroup_2SameGroupWithDiffFaculty() {
            int notifyCount = 0;
            var users = new List<SubscribedUser>();
            users.AddRange(Helpers.CreateUsers(11, 1, "first"));
            users.AddRange(Helpers.CreateUsers(5, 1, "second"));
            users.AddRange(Helpers.CreateUsers(5, 3, "th"));
            users.AddRange(Helpers.CreateUsers(5, 4, "fi"));
            users.AddRange(Helpers.CreateUsers(5, 5, "si"));
            users.AddRange(Helpers.CreateUsers(5, 6, "sev"));

            var subscribedUsersRepo = GetMockSubscribedUsersRepositoryWithUsers(users);
            var mockScheduleDiffService = GetMockScheduleDiffServiceOnlyFirstReturnsTrue();

            var mockUserNotifyService = GetMockIUserNotifyService;
            mockUserNotifyService.Setup(s =>
                    s.Notify(
                        It.Is<SubscribedUser>(user => user.GroupExternalId == 1),
                        It.IsAny<INotifyPayload>()))
                .Callback(() => notifyCount++);

            var mockCheckToNotifyPayloadConverter =
                GetCheckToNotifyPayloadConverterConvertAnyReturns(_emptyNotifyPayload);

            var scheduler = new SchedulerHostedService<ICheckPayload, INotifyPayload>(
                _emptyLogger, subscribedUsersRepo, mockScheduleDiffService.Object, mockUserNotifyService.Object,
                mockCheckToNotifyPayloadConverter, GetOptions());

            await scheduler.StartAsync(CancellationToken.None);
            await Task.Delay(1000);

            Assert.Equal(11, notifyCount);
        }

        private Mock<ICheckScheduleDiffService<ICheckPayload>> GetMockScheduleDiffServiceOnlySecondReturnsTrue() {
            var mockScheduleDiffService = GetMockICheckScheduleDiffServiceCheck;
            mockScheduleDiffService.SetupSequence(
                    service => service.CheckDiff(
                        It.IsAny<string>(),
                        It.IsAny<int>()))
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsTrue)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse);
            return mockScheduleDiffService;
        }

        private Mock<ICheckScheduleDiffService<ICheckPayload>> GetMockScheduleDiffServiceOnlyFirstReturnsTrue() {
            var mockScheduleDiffService = GetMockICheckScheduleDiffServiceCheck;
            mockScheduleDiffService.SetupSequence(
                    service => service.CheckDiff(
                        It.IsAny<string>(),
                        It.IsAny<int>()))
                .ReturnsAsync(_checkPayloadDiffReturnsTrue)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse)
                .ReturnsAsync(_checkPayloadDiffReturnsFalse);
            return mockScheduleDiffService;
        }

        private static List<SubscribedUser> GetDiffGroupSameFacultySubscribedUsers() {
            var users = new List<SubscribedUser>();
            users.AddRange(Helpers.CreateUsers(5, 1, "first"));
            users.AddRange(Helpers.CreateUsers(5, 2, "first"));
            users.AddRange(Helpers.CreateUsers(5, 3, "first"));
            users.AddRange(Helpers.CreateUsers(5, 4, "first"));
            users.AddRange(Helpers.CreateUsers(5, 5, "first"));
            users.AddRange(Helpers.CreateUsers(5, 6, "first"));
            users.AddRange(Helpers.CreateUsers(5, 7, "first"));
            return users;
        }

        private static List<SubscribedUser> GetDiffGroupDiffFacultySubscribedUsers() {
            var users = new List<SubscribedUser>();
            users.AddRange(Helpers.CreateUsers(5, 1, "first"));
            users.AddRange(Helpers.CreateUsers(5, 2, "sec"));
            users.AddRange(Helpers.CreateUsers(5, 3, "th"));
            users.AddRange(Helpers.CreateUsers(5, 4, "frt"));
            users.AddRange(Helpers.CreateUsers(5, 5, "fi"));
            users.AddRange(Helpers.CreateUsers(5, 6, "si"));
            users.AddRange(Helpers.CreateUsers(5, 7, "se"));
            return users;
        }

        private SchedulerHostedService<ICheckPayload, INotifyPayload>
            GetDefaultHostedServiceTestSubscribedUser(List<SubscribedUser> users, Action callback)
        {
            var subscribedUsersRepo = GetMockSubscribedUsersRepositoryWithUsers(users);
            var mockScheduleDiffService = GetCheckScheduleDiffServiceCheckDiffReturns(
                _checkPayloadDiffReturnsTrue);
            var mockUserNotifyService = GetUserNotifyServiceNotifyAnyWithCallback(callback);
            var mockCheckToNotifyPayloadConverter =
                GetCheckToNotifyPayloadConverterConvertAnyReturns(_emptyNotifyPayload);

            return new SchedulerHostedService<ICheckPayload, INotifyPayload>(
                _emptyLogger, subscribedUsersRepo, mockScheduleDiffService, mockUserNotifyService,
                mockCheckToNotifyPayloadConverter, GetOptions());
        }

        private IOptions<RozkladSubscribeServiceOptions> GetOptions()
        {
            return Mock.Of<IOptions<RozkladSubscribeServiceOptions>>(o =>
                o.Value.ProcessPeriod == TimeSpan.FromSeconds(10));
        }
    }
}
