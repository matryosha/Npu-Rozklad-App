using RozkladSubscribeModuleClient.Entities;

namespace RozkladSubscribeModuleClient.Interfaces
{
    internal interface ILastGroupScheduleStorage
    {
        SectionLessons GetSchedule();
        void SetSchedule(SectionLessons section);
    }
}