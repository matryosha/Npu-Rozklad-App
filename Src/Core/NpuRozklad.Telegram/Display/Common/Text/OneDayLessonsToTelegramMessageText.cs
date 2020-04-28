using System;
using System.Collections.Generic;
using System.Text;
using NpuRozklad.Core;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Display.Common.Text
{
    public class OneDayLessonsToTelegramMessageText
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentUserService;
        private readonly ILocalDateService _localDateService;

        private string UserLang => _currentUserService.Language;

        public OneDayLessonsToTelegramMessageText(ILocalizationService localizationService,
            ICurrentTelegramUserService currentUserService, ILocalDateService localDateService)
        {
            _localizationService = localizationService;
            _currentUserService = currentUserService;
            _localDateService = localDateService;
        }

        public string CreateMessage(OneDayLessonsToTelegramMessageTextOptions options)
        {
            var oneDayLessons = options.OneDayLessons;
            var facultyGroup = options.FacultyGroup;
            var lessonsDate = options.LessonsDate;

            return CreateOneDayLessonsTelegramMessage(oneDayLessons, lessonsDate, facultyGroup);
        }

        private string CreateOneDayLessonsTelegramMessage(
            ICollection<Lesson> lessons,
            DateTime date,
            Group facultyGroup)
        {
            var currentDate = _localDateService.LocalDateTime;
            var message = new StringBuilder();
            message.AppendLine($"<b>--{facultyGroup.TypeId}--</b>");
            
            message.AppendLine(
                $"{_localizationService[UserLang, "classes-on"]} " +
                $"<b>{_localizationService[UserLang, date.DayOfWeek, asFullDayName: true]}</b> " +
                $"<code>{date:dd/MM}</code>");
            
            message.AppendLine($"{_localizationService[UserLang, "updated-on"]} {currentDate:HH:mm:ss}");
            message.AppendLine(Environment.NewLine);

            foreach (var lesson in lessons)
            {
                message.AppendLine(GetLessonText(lesson));
                message.AppendLine();
            }

            return message.ToString();
        }

        private string GetLessonText(Lesson lesson)
        {
            var result = new StringBuilder();
            var subgroupInfo = lesson.SubGroup == SubGroup.First
                ? $"1 {_localizationService[UserLang, "group"]}"
                : lesson.SubGroup == SubGroup.Second
                    ? $"2 {_localizationService[UserLang, "group"]}"
                    : "";

            result.AppendLine(GetLessonNumber(lesson.LessonNumber));
            if (lesson.Subject != null && !string.IsNullOrWhiteSpace(lesson.Subject.Name))
                result.AppendLine(GetSubjectNameString(lesson.Subject.Name));

            if (lesson.Lecturer != null && !string.IsNullOrWhiteSpace(lesson.Lecturer.FullName))
                result.AppendLine(GetLecturerNameString(lesson.Lecturer.FullName));

            if (lesson.Classroom != null && !string.IsNullOrWhiteSpace(lesson.Classroom.Name))
                result.AppendLine(GetClassroomNameString(lesson.Classroom.Name));

            if (subgroupInfo != string.Empty) result.AppendLine(GetSubgroupString(subgroupInfo));

            return result.ToString();
        }

        private static string GetSubjectNameString(string subject) => "📘 " + subject;

        private static string GetLecturerNameString(string lecturer) => "👤 " + lecturer;

        private static string GetClassroomNameString(string classroom) => "🚪 " + classroom;

        private static string GetSubgroupString(string subgroup) => "👥 " + subgroup;

        private static string GetLessonNumber(int lesson)
        {
            var result = new StringBuilder();
            result.Append(TextDecoration.LessonNumberToEmoji(lesson));
            result.Append($" <i>{LessonTextHelper.ConvertLessonNumberToText(lesson)}</i>");
            return result.ToString();
        }
    }

    public class OneDayLessonsToTelegramMessageTextOptions
    {
        public DateTime LessonsDate { get; set; }
        public ICollection<Lesson> OneDayLessons { get; set; }
        public Group FacultyGroup { get; set; }
    }
}