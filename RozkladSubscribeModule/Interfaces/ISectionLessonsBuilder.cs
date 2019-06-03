using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISectionLessonsBuilder
    {
        Task<SectionLessons> BuildSection(
            List<DateTime> dates,
            int groupExternalId,
            string facultyShortName);
    }
}