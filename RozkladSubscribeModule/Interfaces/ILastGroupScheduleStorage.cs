using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ILastGroupScheduleStorage
    {
        SectionLessons GetSchedule();
        void SetSchedule(SectionLessons section);
    }
}