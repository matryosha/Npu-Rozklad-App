using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.LessonsProvider.Entities;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider
{
    public class LessonProvider : ILessonsProvider
    {
        private readonly IUnprocessedExtendedLessonsHolder _rawLessonsHolder;
        private readonly IOddDayWeekChecker _oddDayWeekChecker;

        //to options
        private int _deltaGapInDays = -140;

        public LessonProvider(IUnprocessedExtendedLessonsHolder rawLessonsHolder,
            IOddDayWeekChecker oddDayWeekChecker)
        {
            _rawLessonsHolder = rawLessonsHolder;
            _oddDayWeekChecker = oddDayWeekChecker;
        }

        public async Task<ICollection<Lesson>> GetLessonsOnDate(Group facultyGroup, DateTime date)
        {
            var rawLessons =
                await _rawLessonsHolder.GetFacultyUnprocessedLessons(facultyGroup.Faculty).ConfigureAwait(false);

            foreach (var lesson in rawLessons)
            {
                lesson.LessonDate = date;
            }

            /*
            * LEGACY BELOW
            */

            var isOddDay = await _oddDayWeekChecker.IsOddDayWeek(date).ConfigureAwait(false);
            var startPoint = date.AddDays(_deltaGapInDays);

            var resultLessonsList = new List<ExtendedLesson>();

            while (startPoint <= date)
            {
                var point = startPoint;
                var moreRecentLessonsList = rawLessons
                    .Where(lesson => lesson.Group != null && lesson.LessonSetUpDate == point)
                    .ToList();

                if (moreRecentLessonsList.Any())
                {
                    //Doing merging only if current lessonList isn't empty
                    if (resultLessonsList.Any())
                        LessonsMerger.MergeLessonsList(resultLessonsList, moreRecentLessonsList);
                    else
                        resultLessonsList.AddRange(moreRecentLessonsList);
                }

                startPoint = startPoint.AddDays(7);
            }

            var deleteOldLessons = new List<Lesson>(resultLessonsList);

            var currentWeek = isOddDay ? Fraction.Numerator : Fraction.Denominator;

            foreach (var lesson in resultLessonsList)
            {
                int deltaDateTime;
                if (lesson.Fraction == Fraction.None)
                    deltaDateTime = (date - lesson.LessonSetUpDate).Days / 7;
                else
                {
                    deltaDateTime =
                        ((date - lesson.LessonSetUpDate).Days / 7) / 2; //There is might be a problem when number is odd
                }

                if (lesson.LessonCount - deltaDateTime <= 0) deleteOldLessons.Remove(lesson);
            }

            if (currentWeek == Fraction.Numerator)
            {
                return deleteOldLessons.Where(l => l.Fraction == Fraction.None || l.Fraction == currentWeek)
                    .OrderBy(l => l.LessonNumber).ToList();
            }

            if (currentWeek == Fraction.Denominator)
            {
                return deleteOldLessons.Where(l => l.Fraction == Fraction.None || l.Fraction == currentWeek)
                    .OrderBy(l => l.LessonNumber).ToList();
            }

            return deleteOldLessons;
        }
    }
}