using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using RozkladSubscribeModuleClient.Application;
using RozkladSubscribeModuleClient.Entities;
using RozkladSubscribeModuleClient.Interfaces;

namespace RozkladSubscribeModule.Tests
{
    public class SchedulerHostedServiceTestBase
    {
        private protected INotifyPayload _emptyNotifyPayload;
        private protected ICheckPayload _checkPayloadDiffReturnsTrue;
        private protected ICheckPayload _checkPayloadDiffReturnsFalse;
        private protected ILogger<SchedulerHostedService<ICheckPayload, INotifyPayload>> _emptyLogger;
        
        public SchedulerHostedServiceTestBase()
        {
            _emptyNotifyPayload = Mock.Of<INotifyPayload>();
            _checkPayloadDiffReturnsTrue = Mock.Of<ICheckPayload>(payload => payload.IsDiff() == true);
            _checkPayloadDiffReturnsFalse = Mock.Of<ICheckPayload>(payload => payload.IsDiff() == false);
            _emptyLogger = Mock.Of<ILogger<SchedulerHostedService<ICheckPayload, INotifyPayload>>>();
        }

        private protected ISubscribedUsersRepository GetMockSubscribedUsersRepositoryWithUsers(List<SubscribedUser> users)
        {
            var mock = GetMockISubscribedUsersRepository;
            mock.Setup(repository => repository.GetUsers())
                .Returns(users);
            return mock.Object;
        }

        private protected ICheckScheduleDiffService<ICheckPayload> GetCheckScheduleDiffServiceCheckDiffReturns(
            ICheckPayload checkPayload)
        {
            var mock = GetMockICheckScheduleDiffServiceCheck;
            mock.Setup(service => 
                    service.CheckDiff(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(checkPayload);
            return mock.Object;
        }

        private protected IUserNotifyService<INotifyPayload> GetUserNotifyServiceNotifyAnyWithCallback(
            Action callbackAction)
        {
            var mock = GetMockIUserNotifyService;
            mock.Setup(
                    service => service.Notify(
                        It.IsAny<SubscribedUser>(), It.IsAny<INotifyPayload>()))
                .Callback(callbackAction);

            return mock.Object;
        }

        private protected ICheckToNotifyPayloadConverter<ICheckPayload, INotifyPayload>
            GetCheckToNotifyPayloadConverterConvertAnyReturns(INotifyPayload payload)
        {
            var mock = GetMockICheckToNotifyPayloadConverter;

            mock.Setup(converter =>
                converter.Convert(It.IsAny<ICheckPayload>())).Returns(payload);

            return mock.Object;
        }


        #region MocksDelcaration
        private protected Mock<ISubscribedUsersRepository> GetMockISubscribedUsersRepository =>
            new Mock<ISubscribedUsersRepository>();

        private protected Mock<ICheckScheduleDiffService<ICheckPayload>> GetMockICheckScheduleDiffServiceCheck =>
            new Mock<ICheckScheduleDiffService<ICheckPayload>>();

        private protected Mock<IUserNotifyService<INotifyPayload>> GetMockIUserNotifyService =>
            new Mock<IUserNotifyService<INotifyPayload>>();

        private protected Mock<ICheckToNotifyPayloadConverter<ICheckPayload, INotifyPayload>>
            GetMockICheckToNotifyPayloadConverter => 
            new Mock<ICheckToNotifyPayloadConverter<ICheckPayload, INotifyPayload>>();


        #endregion
    }
}