using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NpuRozklad.Core.Entities;
using NpuRozklad.LessonsProvider.Entities;
using Group = NpuRozklad.Core.Entities.Group;

namespace NpuRozklad.LessonsProvider
{
    internal static class RawStringParser
    {
        public static List<CalendarRawItem> DeserializeCalendar(string rawString)
        {
            var result = new List<CalendarRawItem>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                if (rawValue.Count < 1) continue;

                var item = new CalendarRawItem();

                int groupId, lectureId, classroomId, lessonCount, lessonNumber, fraction, subgroup;
                groupId = lectureId = classroomId = lessonCount = lessonNumber = fraction = subgroup = -1;

                try
                {
                    int.TryParse(rawValue[0], out groupId);
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                try
                {
                    int.TryParse(rawValue[2], out lectureId);
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                try
                {
                    int.TryParse(rawValue[3], out classroomId);
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                try
                {
                    int.TryParse(rawValue[5], out lessonCount);
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                try
                {
                    int.TryParse(rawValue[6], out lessonNumber);
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                try
                {
                    int.TryParse(rawValue[7], out fraction);
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                try
                {
                    int.TryParse(rawValue[8], out subgroup);
                }
                catch (ArgumentOutOfRangeException)
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
                catch (ArgumentOutOfRangeException)
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

        public static List<Group> DeserializeGroups(string rawString)
        {
            var result = new List<Group>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                var typeId = rawValue[0];
                var name = Regex.Unescape(rawValue[1]);

                result.Add(new Group(typeId,
                    string.IsNullOrWhiteSpace(name) ? "No name group" : name));
            }

            return result;
        }

        public static List<Lecturer> DeserializeLecturers(string rawString)
        {
            var result = new List<Lecturer>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                var typeId = rawValue[0];
                var name = Regex.Unescape(rawValue[1]);

                result.Add(new Lecturer(typeId, name));
            }

            return result;
        }

        public static List<Classroom> DeserializeClassrooms(string rawString)
        {
            var result = new List<Classroom>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                var typeId = rawValue[0];
                var name = Regex.Unescape(rawValue[1]);

                result.Add(new Classroom(typeId, name));
            }

            return result;
        }

        public static List<Faculty> DeserializeFaculties(string rawString)
        {
            var result = new List<Faculty>();
            var rawValues = GetValues(rawString);

            foreach (var rawValue in rawValues)
            {
                var shortName = Regex.Unescape(rawValue[0]);
                var fullName = Regex.Unescape(rawValue[1]);

                result.Add(new Faculty(shortName, fullName));
            }

            return result;
        }

        public static (DateTime date, bool IsOddDay) GetSettings(string rawString)
        {
            var values = GetValues(rawString);
            var rawValues = JsonConvert.DeserializeObject(rawString) as JObject;

            var oddEvenDaySettingItemAsString = rawValues["response"][4].ToString();
            var separatedStringValues = oddEvenDaySettingItemAsString.Split('|');

            var dateValue = DateTime.Parse(separatedStringValues[0]);
            var boolValue = bool.Parse(separatedStringValues[1]);

            return (dateValue, boolValue);
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