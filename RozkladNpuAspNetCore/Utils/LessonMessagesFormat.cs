using System;
using System.Collections.Generic;
using System.Text;
using NpuTimetableParser;
using RozkladNpuBot.Utils;

namespace RozkladNpuAspNetCore.Utils
{
    public static class LessonMessagesFormat
    {
        public static string CreateOneDayWeekLessonsMessage(List<Lesson> lessons, DateTime currentDate)
        {
            var message = new StringBuilder();
            message.AppendLine(
                $"Пары на *{LessonsUtils.ConvertDayOfWeekToText(currentDate.DayOfWeek)}* `{currentDate:dd/MM}`");
            message.AppendLine(Environment.NewLine);

            foreach (var lesson in lessons)
            {
                message.AppendLine(CreateOneDayLessonMessage(lesson));
                message.AppendLine();
            }

            return message.ToString();
        }

        private static string GetLessonNumber(int lesson)
        {
            var result = new StringBuilder();
            result.Append(LessonsUtils.LessonNumberToEmoji(lesson));
            result.Append(" " + "_"+LessonsUtils.ConvertLessonNumberToText(lesson)+"_");
            return result.ToString();
        }

        public static string CreateOneDayLessonMessage(Lesson lesson)
        {
            var result = new StringBuilder();
            var subgroupInfo = lesson.SubGroup == SubGroup.First ? "1 группа" :
                lesson.SubGroup == SubGroup.Second ? "2 группа" : "";

            result.AppendLine(GetLessonNumber(lesson.LessonNumber));
            if (lesson.Subject != null && !string.IsNullOrWhiteSpace(lesson.Subject.Name))
                result.AppendLine(LessonsUtils.GetSubjectNameString(lesson.Subject.Name));

            if (lesson.Lecturer != null && !string.IsNullOrWhiteSpace(lesson.Lecturer.FullName))
                result.AppendLine(LessonsUtils.GetLecturerNameString(lesson.Lecturer.FullName));

            if (lesson.Classroom != null && !string.IsNullOrWhiteSpace(lesson.Classroom.Name))
                result.AppendLine(LessonsUtils.GetClassroomNameString(lesson.Classroom.Name));

            if (subgroupInfo != string.Empty) result.AppendLine(LessonsUtils.GetSubgroupString(subgroupInfo));

            return result.ToString();
        }
    }
}
