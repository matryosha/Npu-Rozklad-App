using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using RestSharp;

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

    public class CalendarRawItem
    {
        [JsonProperty("0")]
        public int GroupId { get; set; }
        [JsonProperty("1")]
        public string SubjectName { get; set; }
        [JsonProperty("2")]
        public int LectureId { get; set; }
        [JsonProperty("3")]
        public int ClassroomId { get; set; }
        [JsonProperty("4")]
        public int LessonCount { get; set; }
        [JsonProperty("5")]
        public string LessonSetDate { get; set; } //TODO: Deserialize into DateTime at once
        [JsonProperty("6")]
        public int LessonNumber { get; set; }
        [JsonProperty("7")]
        public int Fraction { get; set; }
        [JsonProperty("8")]
        public int SubGroup { get; set; }
    }

    public class NpuParser
    {
        private IRestClient _client;

        public NpuParser(IRestClient client)
        {
            _client = client;
        }

        public NpuParser()
        {
            _client = new RestClient("http://ei.npu.edu.ua");//TODO: extract string
        }

        public List<CalendarRawItem> FillCalendarRawList()
        {
            var calendarRawList = new List<CalendarRawItem>();
            if(_client == null) throw new Exception("client is null");

            var request = new RestRequest("Server.php", Method.POST);

            request.AddParameter("code", "get calendar");
            request.AddParameter("params", "");
            request.AddParameter("loginpass", "");
            request.AddParameter("faculty", "fi");
 
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            
            IRestResponse response = _client.Execute(request);
            RawStringParser parser = new RawStringParser(response.Content);
            calendarRawList = parser.Convert(calendarRawList);
           

            return calendarRawList;
        }

    }

    public class RawStringParser
    {
        private string _text;
        public RawStringParser(string input)
        {
            _text = input;
        }

        public List<CalendarRawItem> Convert(List<CalendarRawItem> collection)
        {
            for (int i = 13; i < _text.Length; i++)
            {
                if (_text[i] == '[')
                {
                    if(_text[i+1] == '[') continue;
                    //inside [ ]
                    i++;
                    int offset = 1; //TODO: safe to remove

                    CalendarRawItem item = new CalendarRawItem();
                    StringBuilder currentText = new StringBuilder();
                    List<string> valuesInBrackets = new List<string>();

                    while (_text[i] != ']')
                    {
                        while (_text[i] == '"') i++;
                        if(_text[i] == ']') continue;
                        while (_text[i] != '"' && _text[i] != ',')
                        {
                            currentText.Append(_text[i]);
                            i++;
                            offset++; 
                        }

//                        if (!String.IsNullOrEmpty(currentText.ToString()))
//                        {
                            valuesInBrackets.Add(currentText.ToString());
                            currentText.Clear();
//                        }

                        if (_text[i] == '"')
                        {
                            i++;
                            offset++;
                        }

                        if (_text[i] == ',')
                        {
                            i++;
                            offset++;
                        }
                    }

                    if( valuesInBrackets.Count < 1) continue;

                    int groupId = -1;
                    int lectureId = -1;
                    int classroomId = -1;
                    int lessoncount = -1;
                    int lessonnumber = -1;
                    int fraction = -1;
                    int subgroup = -1;

                    try
                    {
                        int.TryParse(valuesInBrackets[0], out groupId);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }

                    try
                    {
                        int.TryParse(valuesInBrackets[2], out lectureId);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }

                    try
                    {
                        int.TryParse(valuesInBrackets[3], out classroomId);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }

                    try
                    {
                        int.TryParse(valuesInBrackets[5], out lessoncount);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }

                    try
                    {
                        int.TryParse(valuesInBrackets[6], out lessonnumber);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }

                    try
                    {
                        int.TryParse(valuesInBrackets[7], out fraction);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }

                    try
                    {
                        int.TryParse(valuesInBrackets[8], out subgroup);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }


                    item.GroupId = groupId;
                    try
                    {
                        item.SubjectName = System.Net.WebUtility.HtmlDecode(valuesInBrackets[1]);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                    }
                    item.LectureId = lectureId;
                    item.ClassroomId = classroomId;
                    try
                    {
                        item.LessonSetDate = valuesInBrackets[4];
                    }
                    catch (ArgumentOutOfRangeException e)
                    { }
                    item.LessonCount = lessoncount;
                    item.LessonNumber = lessonnumber;
                    item.Fraction = fraction;
                    item.SubGroup = subgroup;

                    collection.Add(item);

                }
            }

            return collection;
        }
    }
}
