using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NpuTimetableParser;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Application
{
    internal class CheckScheduleDiffService :
        ICheckScheduleDiffService<DefaultCheckPayload>
    {
        private const int MAX_LESSON_NUMBER = 8;
        private readonly ILogger<CheckScheduleDiffService> _logger;
        private readonly ISectionLessonsManager _lessonsManager;
        private readonly IDateTimesForScheduleDiffCheckGiver _checkDatesGiver;

        public CheckScheduleDiffService(
            ILogger<CheckScheduleDiffService> logger,
            ISectionLessonsManager lessonsManager,
            IDateTimesForScheduleDiffCheckGiver checkDatesGiver)
        {
            _logger = logger;
            _lessonsManager = lessonsManager;
            _checkDatesGiver = checkDatesGiver;
        }
        public async Task<DefaultCheckPayload> CheckDiff(string facultyShortName, int groupExternalId)
        {
            _logger.LogDebug($"Checking schedule diff for {facultyShortName}:{groupExternalId}");

            var checkDates = _checkDatesGiver.GetDates();

            var currentSection =
                await _lessonsManager.GetCurrentSectionLessons(
                    groupExternalId, facultyShortName);

            var lastSection =
                _lessonsManager.GetLastSectionLessons(
                    groupExternalId, facultyShortName);

            var result = new DefaultCheckPayload();

            foreach (var date in checkDates)
            {
                var currentLessons = currentSection[date];
                var lastLessons = lastSection[date];

                if (currentLessons.GetHashCode() == lastLessons.GetHashCode())
                    continue;


                for (int lessonNumber = 1; lessonNumber < MAX_LESSON_NUMBER; lessonNumber++)
                {
                    var lessonNumberCurrentLessons = 
                        currentLessons.Where(l => l.LessonNumber == lessonNumber).ToList();

                    var lessonNumberLastLessons = 
                        lastLessons.Where(l => l.LessonNumber == lessonNumber).ToList();

                    if(!lessonNumberCurrentLessons.Any() && !lessonNumberLastLessons.Any())
                        continue;

                    if (lessonNumberCurrentLessons.Any() && lessonNumberLastLessons.Any())
                    {
                        foreach (var fraction in Enum.GetValues(typeof(Fraction)))
                        {
                            foreach (var group in Enum.GetValues(typeof(SubGroup)))
                            {
                                var currentFractionGroupLesson =
                                    lessonNumberCurrentLessons.FirstOrDefault(l =>
                                        l.Fraction == (Fraction) Convert.ToInt32(fraction) &&
                                        l.SubGroup == (SubGroup) Convert.ToInt32(group));

                                var lastFractionGroupLesson =
                                    lessonNumberLastLessons.FirstOrDefault(l =>
                                        l.Fraction == (Fraction) Convert.ToInt32(fraction) &&
                                        l.SubGroup == (SubGroup) Convert.ToInt32(group));


                                if (currentFractionGroupLesson == null)
                                {
                                    if (lastFractionGroupLesson == null)
                                        continue;

                                    // lastFractionGroupLesson -> null
                                    result.AddDateWithNewSchedule(lastFractionGroupLesson.LessonDate);

                                } else
                                {
                                    if (lastFractionGroupLesson != null)
                                    {
                                        if(currentFractionGroupLesson.Subject.Id ==
                                           lastFractionGroupLesson.Subject.Id)
                                            continue;

                                        // lastFractionGroupLesson -> currentFractionGroupLesson
                                        result.AddDateWithNewSchedule(currentFractionGroupLesson.LessonDate);
                                    }

                                    // null -> currentFractionGroupLesson
                                    result.AddDateWithNewSchedule(currentFractionGroupLesson.LessonDate);
                                }
                            }
                        }
                    }
                    else if (lessonNumberCurrentLessons.Any())
                    {
                        //Last schedule does not have <lessonNumber>
                        result.AddDateWithNewSchedule(lessonNumberCurrentLessons.FirstOrDefault().LessonDate);
                    }
                    else
                    {
                        //Current schedule does not have <lessonNumber>
                        result.AddDateWithNewSchedule(lessonNumberLastLessons.FirstOrDefault().LessonDate);
                    }

                }

            }

            return result;
        }
    }
}