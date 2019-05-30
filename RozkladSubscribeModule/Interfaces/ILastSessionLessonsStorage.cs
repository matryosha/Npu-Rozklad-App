using System;
using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ILastSessionLessonsStorage
    {
        SectionLessons Get(
            List<DateTime> dateTimes,
            string facultyShortName,
            int groupExternalId);

        void Set(
            List<DateTime> dateTimes,
            string facultyShortName,
            int groupExternalId,
            SectionLessons sectionLessons);
    }
}