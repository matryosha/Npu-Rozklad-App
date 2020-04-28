using System;

namespace NpuRozklad.Core.Entities
{
    public class Lesson
    {
        public Group Group { get; set; }
        public Subject Subject { get; set; }
        public Classroom Classroom { get; set; }
        public Lecturer Lecturer { get; set; }
        public int LessonNumber { get; set; }
        public Fraction Fraction { get; set; }
        public SubGroup SubGroup { get; set; }
        public DateTime LessonDate { get; set; }

        public override string ToString()
        {
            return LessonNumber + " " + Subject.Name + " F:" + (int)Fraction + " S:" + (int)SubGroup;
        }
    }
}