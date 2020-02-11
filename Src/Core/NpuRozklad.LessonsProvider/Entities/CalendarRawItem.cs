namespace NpuRozklad.Parser.Entities
{
    internal class CalendarRawItem
    {
        public int GroupId { get; set; }
        public string SubjectName { get; set; }
        public int LectureId { get; set; }
        public int ClassroomId { get; set; }
        public int LessonCount { get; set; }
        public string LessonSetDate { get; set; } //TODO: Deserialize into DateTime at once
        public int LessonNumber { get; set; }
        public int Fraction { get; set; }
        public int SubGroup { get; set; }
    }
}