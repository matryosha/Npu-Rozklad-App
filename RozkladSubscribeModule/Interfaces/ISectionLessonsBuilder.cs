using System;
using RozkladSubscribeModuleClient.Entities;

namespace RozkladSubscribeModuleClient.Interfaces
{
    internal interface ISectionLessonsBuilder
    {
        SectionLessons BuildSection(DateTime startDate, DateTime endDate);
    }
}