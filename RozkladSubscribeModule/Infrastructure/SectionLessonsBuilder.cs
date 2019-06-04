using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RozkladNpuBot.Infrastructure;
using RozkladNpuBot.Infrastructure.Interfaces;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Infrastructure
{
    internal class SectionLessonsBuilder :
        ISectionLessonsBuilder
    {
        private readonly ILogger<SectionLessonsBuilder> _logger;
        private readonly ILessonsProvider _lessonsProvider;

        public SectionLessonsBuilder(
            ILogger<SectionLessonsBuilder> logger,
            ILessonsProvider lessonsProvider)
        {
            _logger = logger;
            _lessonsProvider = lessonsProvider;
        }
        public async Task<SectionLessons> BuildSection(List<DateTime> dates, int groupExternalId, string facultyShortName)
        {
            var result = new SectionLessons(facultyShortName, groupExternalId);
            foreach (var dateTime in dates)
            {
                var dateTimeLessons =
                    await _lessonsProvider.GetLessonsOnDate(facultyShortName, groupExternalId, dateTime);
                if (dateTimeLessons == null) 
                    _logger.LogWarning("Returned lessons are empty");

                result.Dates.Add(dateTime);
                result.Lessons.Add(dateTime, dateTimeLessons);
            }

            return result;
        }
    }
}