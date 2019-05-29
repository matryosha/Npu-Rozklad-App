using System;
using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISectionLessonsBuilder
    {
        Task<SectionLessons> BuildSection(
            DateTime startDate, 
            DateTime endDate,
            int groupExternalId,
            string facultyShortName);
    }
}