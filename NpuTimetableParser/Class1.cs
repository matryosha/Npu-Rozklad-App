using System;
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

    internal class CalendarRawItem
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
            _client = new RestClient("http://ei.npu.edu.ua/Server.php");//TODO: extract string
        }


    }
}
