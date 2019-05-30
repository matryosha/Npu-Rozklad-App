using System.Collections.Generic;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Application
{
    internal class DefaultCheckToNotifyPayloadConverter :
        ICheckToNotifyPayloadConverter<DefaultCheckPayload, DefaultNotifyPayload>
    {
        public DefaultNotifyPayload Convert(DefaultCheckPayload checkPayload)
        {
            return new DefaultNotifyPayload
            {
                UpdatedLessons =
                    new List<UpdatedLesson>(checkPayload.UpdatedLessons)
            };
        }
    }
}