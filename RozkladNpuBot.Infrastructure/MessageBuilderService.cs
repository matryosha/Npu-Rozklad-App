using System;
using System.Collections.Generic;
using System.Text;
using NpuTimetableParser;
using RozkladNpuBot.Infrastructure;
using RozkladNpuBot.Infrastructure.Interfaces;

namespace RozkladNpuBot.Application.Services
{
    public class MessageBuilderService
    {
        private readonly ILocalizationService _localization;

        public MessageBuilderService(
            ILocalizationService localization)
        {
            _localization = localization;
        }

        public string OneDayClassesMessage(
            List<Lesson> lessons,
            DateTime currentDate,
            Group group)
        {
            var message = new StringBuilder();
            message.AppendLine($"*--{group.ShortName}--*");
            message.AppendLine(
                $"{_localization["ua", "classes-on"]} *{ConvertDayOfWeekToText(currentDate.DayOfWeek)}* `{currentDate:dd/MM}`");
            message.AppendLine($"{_localization["ua", "updated-on"]} {DateTime.Now.ToLocal():HH:mm:ss}");
            message.AppendLine(Environment.NewLine);

            foreach (var lesson in lessons)
            {
                message.AppendLine(CreateOneDayLessonMessage(lesson));
                message.AppendLine();
            }

            return message.ToString();
        }

        public string CreateOneDayLessonMessage(Lesson lesson)
        {
            var result = new StringBuilder();
            var subgroupInfo = lesson.SubGroup == SubGroup.First 
                ? $"1 {_localization["ua", "group"]}" 
                : lesson.SubGroup == SubGroup.Second 
                    ? $"2 {_localization["ua", "group"]}" 
                    : "";

            result.AppendLine(GetLessonNumber(lesson.LessonNumber));
            if (lesson.Subject != null && !String.IsNullOrWhiteSpace(lesson.Subject.Name))
                result.AppendLine(GetSubjectNameString(lesson.Subject.Name));

            if (lesson.Lecturer != null && !String.IsNullOrWhiteSpace(lesson.Lecturer.FullName))
                result.AppendLine(GetLecturerNameString(lesson.Lecturer.FullName));

            if (lesson.Classroom != null && !String.IsNullOrWhiteSpace(lesson.Classroom.Name))
                result.AppendLine(GetClassroomNameString(lesson.Classroom.Name));

            if (subgroupInfo != String.Empty) result.AppendLine(GetSubgroupString(subgroupInfo));

            return result.ToString();
        }

        public string ConvertDayOfWeekToText(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return _localization["ua", "monday"];
                case DayOfWeek.Tuesday:
                    return _localization["ua", "tuesday"];
                case DayOfWeek.Wednesday:
                    return _localization["ua", "wednesday"];
                case DayOfWeek.Thursday:
                    return _localization["ua", "thursday"];
                case DayOfWeek.Friday:
                    return _localization["ua", "friday"];
                case DayOfWeek.Saturday:
                    return _localization["ua", "saturday"];
                case DayOfWeek.Sunday:
                    return _localization["ua", "sunday"];
            }

            return "??";
        }

        private static string GetSubjectNameString(string subject) => "📘 " + subject;
        
        private static string GetLecturerNameString(string lecturer) => "👤 " + lecturer;
        
        private static string GetClassroomNameString(string classroom) => "🚪 " + classroom;
        
        private static string GetSubgroupString(string subgroup) => "👥 " + subgroup;

        private static string ConvertLessonNumberToText(int number)
        {
            switch (number)
            {
                case 1:
                    return "8:00 - 9:20";
                case 2:
                    return "9:30 - 10:50";
                case 3:
                    return "11:00 - 12:20";
                case 4:
                    return "12:30 - 13:50";
                case 5:
                    return "14:00 - 15:20";
                case 6:
                    return "15:30 - 16:50";
                case 7:
                    return "17:00 - 18:20";
                default: return number.ToString();
            }
        }

        private static string GetLessonNumber(int lesson)
        {
            var result = new StringBuilder();
            result.Append((string)LessonNumberToEmoji(lesson));
            result.Append(" " + "_" + ConvertLessonNumberToText(lesson) + "_");
            return result.ToString();
        }

        private static string LessonNumberToEmoji(int num)
        {
            switch (num)
            {
                case 1: return "1️⃣";
                case 2: return "2️⃣";
                case 3: return "3️⃣";
                case 4: return "4️⃣";
                case 5: return "5️⃣";
                case 6: return "6️⃣";
                case 7: return "7️⃣";
                case 8: return "8️⃣";
                case 9: return "9️⃣";
                default: return "🦄";
            }
        }
    }
}
