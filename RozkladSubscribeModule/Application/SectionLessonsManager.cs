using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Application
{
    internal class SectionLessonsManager :
        ISectionLessonsManager
    {
        private readonly ILogger<SectionLessonsManager> _logger;
        private readonly IDateTimesForScheduleDiffCheckGiver _dateTimesForScheduleDiff;
        private readonly ISectionLessonsBuilder _sectionLessonsBuilder;
        private readonly ILastSessionLessonsStorage _lastSessionLessonsStorage;

        public SectionLessonsManager(
            ILogger<SectionLessonsManager> logger,
            IDateTimesForScheduleDiffCheckGiver dateTimesForScheduleDiff,
            ISectionLessonsBuilder sectionLessonsBuilder,
            ILastSessionLessonsStorage lastSessionLessonsStorage)
        {
            _logger = logger;
            _dateTimesForScheduleDiff = dateTimesForScheduleDiff;
            _sectionLessonsBuilder = sectionLessonsBuilder;
            _lastSessionLessonsStorage = lastSessionLessonsStorage;
        }
        public Task<SectionLessons> GetCurrentSectionLessons(int groupExternalId, string facultyShortName)
        {
            return _sectionLessonsBuilder.BuildSection(
                _dateTimesForScheduleDiff.GetDates(), groupExternalId, facultyShortName);
        }

        public SectionLessons GetLastSectionLessons(int groupExternalId, string facultyShortName,
            SectionLessons currentSectionLessons = null)
        {
            var sectionLessons =
                _lastSessionLessonsStorage.Get(
                    _dateTimesForScheduleDiff.GetDates(), facultyShortName, groupExternalId);

            if (sectionLessons != null)
                return sectionLessons;

            if (currentSectionLessons != null)
                _lastSessionLessonsStorage.Set(
                    _dateTimesForScheduleDiff.GetDates(), facultyShortName, groupExternalId, currentSectionLessons);

            return new SectionLessons(facultyShortName, groupExternalId);
        }

        public void UpdateLastSection(SectionLessons sectionLessons)
        {
            throw new NotImplementedException();
        }
    }
}
