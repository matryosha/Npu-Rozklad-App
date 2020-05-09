using System;

namespace NpuRozklad.LessonsProvider.Entities
{
    internal class CalendarRawItem
    {
        //todo to string
        public string GroupId { get; set; }
        public string SubjectName { get; set; }
        public string LecturerId { get; set; }
        public string ClassroomId { get; set; }
        public string LessonCount { get; set; }
        public DateTime LessonSetDate { get; set; } //TODO: Deserialize into DateTime at once
        public string LessonNumber { get; set; }
        public int Fraction { get; set; }
        public int SubGroup { get; set; }
    }
}