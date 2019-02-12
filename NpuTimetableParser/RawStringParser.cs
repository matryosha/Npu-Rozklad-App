using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NpuTimetableParser
{
    public class RawStringParser
    {
        public List<CalendarRawItem> DeserializeCalendar(string rawString)
        {
            var result = new List<CalendarRawItem>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                if (rawValue.Count < 1) continue;

                var item = new CalendarRawItem();

                var groupId = -1;
                var lectureId = -1;
                var classroomId = -1;
                var lessonCount = -1;
                var lessonNumber = -1;
                var fraction = -1;
                var subgroup = -1;

                try
                {
                    int.TryParse(rawValue[0], out groupId);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                try
                {
                    int.TryParse(rawValue[2], out lectureId);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                try
                {
                    int.TryParse(rawValue[3], out classroomId);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                try
                {
                    int.TryParse(rawValue[5], out lessonCount);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                try
                {
                    int.TryParse(rawValue[6], out lessonNumber);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                try
                {
                    int.TryParse(rawValue[7], out fraction);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                try
                {
                    int.TryParse(rawValue[8], out subgroup);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                item.GroupId = groupId;
                try
                {
                    item.SubjectName = Regex.Unescape(rawValue[1]);
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                item.LectureId = lectureId;
                item.ClassroomId = classroomId;
                try
                {
                    item.LessonSetDate = rawValue[4];
                }
                catch (ArgumentOutOfRangeException e)
                {
                }

                item.LessonCount = lessonCount;
                item.LessonNumber = lessonNumber;
                item.Fraction = fraction;
                item.SubGroup = subgroup;

                result.Add(item);
            }

            return result;
        }

        public List<Group> DeserializeGroups(string rawString)
        {
            var result = new List<Group>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                var item = new Group
                {
                    ExternalId = int.Parse(rawValue[0]),
                    ShortName = Regex.Unescape(rawValue[1])
                };
                result.Add(item);
            }

            return result;
        }

        public List<Lecturer> DeserializeLecturers(string rawString)
        {
            var result = new List<Lecturer>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                var item = new Lecturer
                {
                    ExternalId = int.Parse(rawValue[0]),
                    FullName = Regex.Unescape(rawValue[1])
                };
                result.Add(item);
            }

            return result;
        }

        public List<Classroom> DeserializeClassrooms(string rawString)
        {
            var result = new List<Classroom>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                var item = new Classroom
                {
                    ExternalId = int.Parse(rawValue[0]),
                    Name = Regex.Unescape(rawValue[1])
                };
                result.Add(item);
            }

            return result;
        }

        public List<Faculty> DeserializeFaculties(string rawString)
        {
            var result = new List<Faculty>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                var item = new Faculty
                {
                    ShortName = Regex.Unescape(rawValue[0]),
                    FullName = Regex.Unescape(rawValue[1])
                };
                result.Add(item);
            }

            return result;
        }

        private static IEnumerable<List<string>> GetValues(string rawString)
        {
            var values = new List<List<string>>();
            for (var i = 13; i < rawString.Length; i++)
                if (rawString[i] == '[')
                {
                    if (rawString[i + 1] == '[') continue;
                    //inside [ ]
                    i++;

                    var currentText = new StringBuilder();
                    var valuesInBrackets = new List<string>();

                    while (rawString[i] != ']')
                    {
                        while (rawString[i] == '"') i++;
                        if (rawString[i] == ']') continue;
                        while (rawString[i] != '"' && rawString[i] != ',')
                        {
                            currentText.Append(rawString[i]);
                            i++;
                        }

                        valuesInBrackets.Add(currentText.ToString());
                        currentText.Clear();

                        if (rawString[i] == '"') i++;

                        if (rawString[i] == ',') i++;
                    }

                    values.Add(valuesInBrackets);
                }

            return values;
        }
    }
}
