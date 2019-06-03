using System.Collections.Generic;
using RozkladSubscribeModule.Infrastructure.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Entities
{
    internal class DefaultNotifyPayload : INotifyPayload
    {
        public List<UpdatedLesson> UpdatedLessons { get; set; }
    }
}