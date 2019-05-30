using System;
using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ILastGroupScheduleStorage
    {
        SectionLessons GetSchedule(
            List<DateTime> dateTimes,
            string facultyShortName,
            int groupExternalId);

        void SetSchedule(
            List<DateTime> dateTimes,
            string facultyShortName,
            int groupExternalId,
            SectionLessons sectionLessons);
    }
}