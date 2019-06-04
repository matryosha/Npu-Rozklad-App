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

        //Remove faculty and group and get them from section
        void Set(
            List<DateTime> dateTimes,
            string facultyShortName,
            int groupExternalId,
            SectionLessons sectionLessons);
    }
}