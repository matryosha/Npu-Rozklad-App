using System;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISectionLessonsBuilder
    {
        SectionLessons BuildSection(DateTime startDate, DateTime endDate);
    }
}