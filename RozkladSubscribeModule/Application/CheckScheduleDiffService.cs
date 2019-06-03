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
        private const int MaxLessonNumber = 8;
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
                    groupExternalId, facultyShortName, currentSection);

            var result = new DefaultCheckPayload();

            if (!lastSection.Lessons.Any())
                return result;

            foreach (var date in checkDates)
            {
                var currentLessons = currentSection[date];
                var lastLessons = lastSection[date];

                if (currentLessons.GetHashCode() == lastLessons.GetHashCode())
                    continue;


                for (int lessonNumber = 1; lessonNumber < MaxLessonNumber; lessonNumber++)
                {
                    var lessonNumberNewLessons = 
                        currentLessons.Where(l => l.LessonNumber == lessonNumber).ToList();

                    var lessonNumberOldLessons = 
                        lastLessons.Where(l => l.LessonNumber == lessonNumber).ToList();

                    if(!lessonNumberNewLessons.Any() && !lessonNumberOldLessons.Any())
                        continue;

                    if (lessonNumberNewLessons.Any() && lessonNumberOldLessons.Any())
                    {
                        foreach (var fraction in Enum.GetValues(typeof(Fraction)))
                        {
                            foreach (var group in Enum.GetValues(typeof(SubGroup)))
                            {
                                var newFractionSubgroupLesson =
                                    lessonNumberNewLessons.FirstOrDefault(l =>
                                        l.Fraction == (Fraction) Convert.ToInt32(fraction) &&
                                        l.SubGroup == (SubGroup) Convert.ToInt32(group));

                                var oldFractionSubgroupLesson =
                                    lessonNumberOldLessons.FirstOrDefault(l =>
                                        l.Fraction == (Fraction) Convert.ToInt32(fraction) &&
                                        l.SubGroup == (SubGroup) Convert.ToInt32(group));


                                if (newFractionSubgroupLesson == null)
                                {
                                    if (oldFractionSubgroupLesson == null)
                                        continue;

                                    // lastFractionGroupLesson -> null
                                    result.AddDeletedLesson(oldFractionSubgroupLesson);

                                } else
                                {
                                    if (oldFractionSubgroupLesson != null)
                                    {
                                        if(newFractionSubgroupLesson.Subject.Id ==
                                           oldFractionSubgroupLesson.Subject.Id)
                                            continue;

                                        // lastFractionGroupLesson -> currentFractionGroupLesson
                                        result.AddReplacedLesson(
                                            oldFractionSubgroupLesson ,
                                            newFractionSubgroupLesson);
                                    }

                                    // null -> currentFractionGroupLesson
                                    result.AddNewLesson(newFractionSubgroupLesson);
                                }
                            }
                        }
                    }
                    else if (lessonNumberNewLessons.Any())
                    {
                        //Last schedule does not have <lessonNumber>
                        foreach (var lessonNumberCurrentLesson in lessonNumberNewLessons)
                        {
                            result.AddNewLesson(lessonNumberCurrentLesson);
                        }
                    }
                    else
                    {
                        //Current schedule does not have <lessonNumber>
                        foreach (var lessonNumberLastLesson in lessonNumberOldLessons)
                        {
                            result.AddDeletedLesson(lessonNumberLastLesson);
                        }
                    }

                }

            }

            return result;
        }
    }
}