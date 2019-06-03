using System;
using System.Collections.Generic;
using NpuTimetableParser;

namespace RozkladSubscribeModule.Entities
{
    internal class SectionLessons
    {
        public SectionLessons(string facultyShortName, int groupExternalId)
        {
            FacultyShortName = facultyShortName;
            GroupExternalId = groupExternalId;
            Lessons = new Dictionary<DateTime, List<Lesson>>();
            Dates = new List<DateTime>();
        }

        public List<Lesson> this[DateTime date]
        {
            get
            {
                if (Lessons.TryGetValue(date, out var result))
                    return result;
                return new List<Lesson>();
            }
        }

        public string FacultyShortName { get; }
        public int GroupExternalId { get; }
        [Obsolete]
        public List<DateTime> Dates { get; set; }
        public Dictionary<DateTime, List<Lesson>> Lessons { get; set; }
    }
}