﻿using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISectionLessonsManager
    {
        Task<SectionLessons> GetCurrentSectionLessons(
            int groupExternalId,
            string facultyShortName);

        SectionLessons GetLastSectionLessons(
            int groupExternalId,
            string facultyShortName,
            SectionLessons currentSectionLessons = null);

        void UpdateLastSection(
            SectionLessons sectionLessons);
    }
}