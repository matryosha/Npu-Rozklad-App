using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NpuRozklad.Core.Entities;
using NpuRozklad.LessonsProvider.Entities;
using NpuRozklad.LessonsProvider.Exceptions;
using Group = NpuRozklad.Core.Entities.Group;

namespace NpuRozklad.LessonsProvider
{
    internal static class RawStringParser
    {
        private const string NpuDateFormat = "yyyy-MM-dd";
        public static int NumberOfCalendarItemsToTake = 1000;

        public static List<CalendarRawItem> DeserializeCalendar(string rawString)
        {
            try
            {
                var result = new List<CalendarRawItem>();
            
                var jArrayWithCalendarValues = (JArray) JObject.Parse(rawString)["response"][0];
                var jTokenList = jArrayWithCalendarValues.Skip(jArrayWithCalendarValues.Count - NumberOfCalendarItemsToTake);
            
                foreach (var token in jTokenList)
                {
                    if (!token.HasValues) continue;
                    var tokenAsJArray = token as JArray;
                    CalendarRawItem item;
                    try
                    {
                        item = new CalendarRawItem
                        {
                            GroupId = tokenAsJArray[0].Value<string>(),
                            LecturerId = tokenAsJArray[2].Value<string>(),
                            ClassroomId = tokenAsJArray[3].Value<string>(),
                            LessonCount = tokenAsJArray[5].Value<string>(),
                            LessonNumber = tokenAsJArray[6].Value<string>(),
                            Fraction = tokenAsJArray[7].Value<int>(),
                            SubGroup = tokenAsJArray[8].Value<int>(),
                            SubjectName = tokenAsJArray[1].Value<string>(),
                            LessonSetDate = DateTime.ParseExact(tokenAsJArray[4].Value<string>(), NpuDateFormat, null)
                        };

                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    result.Add(item);

                }
                return result;
            }
            catch (Exception e)
            {
                throw new DeserializationException("Deserializing calendar error", e);
            }
        }

        public static List<Group> DeserializeGroups(string rawString, Faculty groupFaculty)
        {
            try
            {
                var result = new List<Group>();
            
                if (rawString == null) return result;
            
                var rawValues = GetValues(rawString);

                foreach (var rawValue in rawValues)
                {
                    var typeId = rawValue[0];
                    var name = Regex.Unescape(rawValue[1]);

                    result.Add(new Group(typeId,
                        string.IsNullOrWhiteSpace(name) ? "No name group" : name,
                        groupFaculty));
                }

                return result;
            }
            catch (Exception e)
            {
                throw new DeserializationException("Deserializing groups error", e);
            }
        }

        public static List<Lecturer> DeserializeLecturers(string rawString)
        {
            try
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
            catch (Exception e)
            {
                throw new DeserializationException("Deserializing lecturers error", e);
            }
        }

        public static List<Classroom> DeserializeClassrooms(string rawString)
        {
            try
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
            catch (Exception e)
            {
                throw new DeserializationException("Deserializing classrooms error", e);
            }
        }

        public static List<Faculty> DeserializeFaculties(string rawString)
        {
            try
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
            catch (Exception e)
            {
                throw new DeserializationException("Deserializing faculties error", e);
            }
        }

        public static (DateTime date, bool IsOddDay) DeserializeSettings(string rawString)
        {
            try
            {
                var rawValues = JsonConvert.DeserializeObject(rawString) as JObject;

                var oddEvenDaySettingItemAsString = rawValues["response"][4].ToString();
                var separatedStringValues = oddEvenDaySettingItemAsString.Split('|');

                var dateValue = DateTime.Parse(separatedStringValues[0]);
                var boolValue = bool.Parse(separatedStringValues[1]);

                return (dateValue, boolValue);
            }
            catch (Exception e)
            {
                throw new DeserializationException("Deserializing settings error", e);
            }
        }

        // Start using JObject?
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