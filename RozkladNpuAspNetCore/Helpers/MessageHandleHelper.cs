﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NpuTimetableParser;
using Telegram.Bot.Types.ReplyMarkups;

namespace RozkladNpuAspNetCore.Helpers
{
    public static class MessageHandleHelper
    {
        public static string CreateOneDayWeekLessonsMessage(List<Lesson> lessons, DateTime currentDate)
        {
            var message = new StringBuilder();
            message.AppendLine(
                $"Пары на *{ConvertDayOfWeekToText(currentDate.DayOfWeek)}* `{currentDate:dd/MM}`");
            message.AppendLine(Environment.NewLine);

            foreach (var lesson in lessons)
            {
                message.AppendLine(CreateOneDayLessonMessage(lesson));
                message.AppendLine();
            }

            return message.ToString();
        }

        public static string CreateOneDayLessonMessage(Lesson lesson)
        {
            var result = new StringBuilder();
            var subgroupInfo = lesson.SubGroup == SubGroup.First ? "1 группа" :
                lesson.SubGroup == SubGroup.Second ? "2 группа" : "";

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

        public static string ConvertLessonNumberToText(int number)
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
                    return "воу 7 пара, эх **nema**";
                default: return number.ToString();
            }
        }

        public static string LessonNumberToEmoji(int num)
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

        public static string ConvertDayOfWeekToText(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Понедельник";
                case DayOfWeek.Tuesday:
                    return "Вторник";
                case DayOfWeek.Wednesday:
                    return "Среду";
                case DayOfWeek.Thursday:
                    return "Четверг";
                case DayOfWeek.Friday:
                    return "Пятницу";
                case DayOfWeek.Saturday:
                    return "Субботу";
                case DayOfWeek.Sunday:
                    return "Воскресение";
            }

            return "??";
        }

        public static string GetSubjectNameString(string subject) => "📘 " + subject;

        public static string GetLecturerNameString(string lecturer) => "👤 " + lecturer;

        public static string GetClassroomNameString(string classroom) => "🚪 " + classroom;

        public static string GetSubgroupString(string subgroup) => "👥 " + subgroup;

        public static ReplyKeyboardMarkup GetFacultiesReplyKeyboardMarkup(List<Faculty> faculties)
        {
            var rows = new List<List<KeyboardButton>>();
            foreach (var faculty in faculties)
            {
                var row = new List<KeyboardButton>();
                row.Add(faculty.FullName);
                rows.Add(row);
            }

            rows.Add(new List<KeyboardButton>
            {
                "Menu"
            });

            return new ReplyKeyboardMarkup(rows);
        }

        public static ReplyKeyboardMarkup GetGroupsReplyKeyboardMarkup(List<Group> groups)
        {
            var groupsRow = new
                List<List<KeyboardButton>>();
            foreach (var group in groups)
            {
                var row = new List<KeyboardButton> {group.ShortName};
                groupsRow.Add(row);
            }

            return new ReplyKeyboardMarkup(groupsRow);
        }

        public static ReplyKeyboardMarkup GetMainMenuReplyKeyboardMarkup()
        {
            return new[]
            {
                new []{ "Schedule" },
                new []{ "Настройки" }
            };
        }

        public static ReplyKeyboardMarkup GetScheduleActionsReplyKeyboardMarkup()
        {
            return new[]
            {
                new[] {"Расписание на сегодня"},
                new[] {"Расписание на завтра"},
                new[] {"Расписание на неделю"},
                new[] {"Назад к меню"}
            };
        }

        public static ReplyKeyboardMarkup GetSettingsActionsReplyKeyboardMarkup()
        {
            return new[]
            {
                new[] {"Сбросить настройки"},
                new[] {"Назад к меню"}
            };
        }

        public static ReplyKeyboardMarkup GetConfirmButtonsReplyKeyboardMarkup() => new[]
        {
            new[] {"Да"},
            new[] {"Нет"}
        };

        public static ReplyKeyboardMarkup GetWeekScheduleActionsReplyKeyboardMarkup()
        {
            return new[]
            {
                new[] {"На текущую неделю"},
                new[] {"На следующую неделю"},
                new[] {"Назад к меню"}
            };
        }

        private static string GetLessonNumber(int lesson)
        {
            var result = new StringBuilder();
            result.Append((string)LessonNumberToEmoji(lesson));
            result.Append(" " + "_" + ConvertLessonNumberToText(lesson) + "_");
            return result.ToString();
        }
    }
}
