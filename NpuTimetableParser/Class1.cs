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
    }

    public class Group
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
    }

    public class Lecturer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }

    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Lesson
    {
        public Group Group { get; set; }
        public Subject Subject { get; set; }
        public Classroom Classroom { get; set; }
        public int LessonNumber { get; set; }
        public DateTime LessonDate { get; set; }
        public Fraction Fraction { get; set; }
        public int LessonCount { get; set; }

    }



}
