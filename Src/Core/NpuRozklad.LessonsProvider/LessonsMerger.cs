using System.Collections.Generic;
using System.Linq;
using NpuRozklad.Core.Entities;
using NpuRozklad.LessonsProvider.Entities;

namespace NpuRozklad.LessonsProvider
{
    public static class LessonsMerger
    {
         /// <summary>
        /// Resolve all conflicts and put new lesson in right place
        /// </summary>
        /// <param name="resultLessonsList"></param>
        /// <param name="moreRecentLessonsList"></param>
        public static void MergeLessonsList(List<ExtendedLesson> resultLessonsList, ICollection<ExtendedLesson> moreRecentLessonsList)
        {
            foreach (var newLesson in moreRecentLessonsList)
            {
                //Check if there is a lesson with the same lesson number
                var sameLessonsNumber = resultLessonsList.Where(l => l.LessonNumber == newLesson.LessonNumber).ToList();
                if (!sameLessonsNumber.Any())
                {
                    resultLessonsList.Add(newLesson);
                    continue;
                }
                foreach (var oldLessonWithSameNumber in sameLessonsNumber)
                {
                    if (resultLessonsList.Contains(newLesson)) continue;
                    if (newLesson.Fraction == Fraction.None &&
                        newLesson.SubGroup == SubGroup.None)
                    {
                        //Remove all lessons with that lesson number
                        resultLessonsList.RemoveAll(l => l.LessonNumber == newLesson.LessonNumber);
                        resultLessonsList.Add(newLesson);
                        continue;
                    }
                    if (oldLessonWithSameNumber.Fraction == newLesson.Fraction &&
                        newLesson.SubGroup == SubGroup.None)
                    {
                        resultLessonsList.RemoveAll(l => l.LessonNumber == newLesson.LessonNumber &&
                                                         l.Fraction == newLesson.Fraction);
                        resultLessonsList.Add(newLesson);
                        continue;
                    }
                    if (oldLessonWithSameNumber.Fraction == newLesson.Fraction &&
                        oldLessonWithSameNumber.SubGroup == newLesson.SubGroup &&
                        oldLessonWithSameNumber.SubGroup != SubGroup.None)
                    {
                        ReplaceLesson(resultLessonsList, newLesson, oldLessonWithSameNumber);
                        continue;
                    }
                    if (oldLessonWithSameNumber.Fraction == newLesson.Fraction &&
                        oldLessonWithSameNumber.SubGroup == newLesson.SubGroup)
                    {
                        ReplaceLesson(resultLessonsList, newLesson, oldLessonWithSameNumber);
                        continue;
                    }
                    resultLessonsList.Add(newLesson);
                }
            }
        }
        public static void ReplaceLesson(List<ExtendedLesson> list, ExtendedLesson newLesson, ExtendedLesson oldLesson)
        {
            list.Remove(oldLesson);
            list.Add(newLesson);
        }
    }
}