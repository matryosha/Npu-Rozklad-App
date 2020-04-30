using System;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.LessonsProvider.Entities
{
    internal class ExtendedLesson : Lesson
    {
        /// <summary>
        /// Contains date when lesson has been set up. Not actual date
        /// </summary>
        public DateTime LessonSetUpDate { get; set; }
        /// <summary>
        /// Indicates how many times lesson must appear in timetable
        /// </summary>
        public int LessonCount { get; set; }
    }
}