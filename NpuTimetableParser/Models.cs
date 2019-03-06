using System;

namespace NpuTimetableParser
{
    public enum Fraction
    {
        None,
        Numerator,
        Denominator
    }

    public enum SubGroup
    {
        None,
        First,
        Second
    }

    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class Group
    {
        public int ExternalId { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public override string ToString()
        {
            return ShortName;
        }
    }

    public class Lecturer
    {
        public int ExternalId { get; set; }
        public string FullName { get; set; }
        public override string ToString()
        {
            return FullName;
        }
    }

    public class Classroom
    {
        public int ExternalId { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }

    public class Lesson
    {
        public Group Group { get; set; }
        public Subject Subject { get; set; }
        public Classroom Classroom { get; set; }
        public Lecturer Lecturer { get; set; }
        public int LessonNumber { get; set; }
        /// <summary>
        /// Contains date when lesson has been set up. Not actual date
        /// </summary>
        public DateTime LessonDate { get; set; }
        public Fraction Fraction { get; set; }
        public SubGroup SubGroup { get; set; }
        /// <summary>
        /// Indicates how many times lesson must appear in timetable
        /// </summary>
        public int LessonCount { get; set; }

        public override string ToString()
        {
            return LessonNumber + " " + Subject.Name + " F:" + (int)Fraction + " S:" + (int)SubGroup;
        }
    }

    public class CalendarRawItem
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

    public class Faculty
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public override string ToString()
        {
            return FullName + " " + ShortName;
        }
    }
}
