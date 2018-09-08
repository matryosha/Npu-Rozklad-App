using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using Decode = System.Text.RegularExpressions.Regex;

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
        public int ExternalId { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
    }

    public class Lecturer
    {
        public int ExternalId { get; set; }
        public string FullName { get; set; }
    }

    public class Classroom
    {
        public int ExternalId { get; set; }
        public string Name { get; set; }
    }

    public class Lesson
    {
        public Group Group { get; set; }
        public Subject Subject { get; set; }
        public Classroom Classroom { get; set; }
        public Lecturer Lecturer { get; set; }
        public int LessonNumber { get; set; }
        public DateTime LessonDate { get; set; }
        public Fraction Fraction { get; set; }
        public SubGroup SubGroup { get; set; }
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
        private RawStringParser _rawParser;
        private List<Classroom> _classrooms;
        private List<Group> _groups;
        private List<Lecturer> _lecturers;
        private List<CalendarRawItem> _calendarRawList;
        private List<Lesson> _lessons;

        public NpuParser(IRestClient client)
        {
            _client = client;
            _rawParser = new RawStringParser();
        }

        public NpuParser()
        {
            _client = new RestClient("http://ei.npu.edu.ua");//TODO: extract string
            _rawParser = new RawStringParser();
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
            calendarRawList = _rawParser.ConvertCalendarRaw(calendarRawList, response.Content);  

            return calendarRawList;
        }

        public List<Group> FillGroupList()
        {
            var groupList = new List<Group>();
            if (_client == null) throw new Exception("client is null");

            var request = new RestRequest("Server.php", Method.POST);

            request.AddParameter("code", "get groups");
            request.AddParameter("params", "");
            request.AddParameter("loginpass", "");
            request.AddParameter("faculty", "fi");

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = _client.Execute(request);
            groupList = _rawParser.ConvertGroupRaw(groupList, response.Content);

            return groupList;
        }

        public List<Lecturer> FillLecturersList()
        {
            var lecturesList = new List<Lecturer>();
            if (_client == null) throw new Exception("client is null");

            var request = new RestRequest("Server.php", Method.POST);

            request.AddParameter("code", "get lectors");
            request.AddParameter("params", "");
            request.AddParameter("loginpass", "");
            request.AddParameter("faculty", "fi");

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = _client.Execute(request);
            lecturesList = _rawParser.ConvertLecturersRaw(lecturesList, response.Content);

            return lecturesList;
        }

        public List<Classroom> FillClassroomsList()
        {
            var classroomsList = new List<Classroom>();
            if (_client == null) throw new Exception("client is null");

            var request = new RestRequest("Server.php", Method.POST);

            request.AddParameter("code", "get auditories");
            request.AddParameter("params", "");
            request.AddParameter("loginpass", "");
            request.AddParameter("faculty", "fi");

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = _client.Execute(request);
            classroomsList = _rawParser.ConvertClassroomsRaw(classroomsList, response.Content);

            return classroomsList;
        }

        public void CreateLessonsList()
        {
            _calendarRawList = FillCalendarRawList();
            _groups = FillGroupList();
            _lecturers = FillLecturersList();
            _classrooms = FillClassroomsList();
            _lessons = new List<Lesson>();

            foreach (var calendarRawItem in _calendarRawList)
            {
                var lesson = new Lesson();
                //Set lesson date
                if (!string.IsNullOrEmpty(calendarRawItem.LessonSetDate))
                {
                    try
                    {
                        lesson.LessonDate = DateTime.Parse(calendarRawItem.LessonSetDate);
                    }
                    catch (FormatException e)
                    {
                        //TODO: LOG
                    }      
                }
                //Set subject
                if (!string.IsNullOrEmpty(calendarRawItem.SubjectName))
                {
                    var subject = new Subject();
                    subject.Name = calendarRawItem.SubjectName;
                    lesson.Subject = subject;
                }
                //Set group
                if (calendarRawItem.GroupId != -1)
                {
                    lesson.Group = _groups.FirstOrDefault(a => a.ExternalId == calendarRawItem.GroupId);
                }
                //Set lecture
                if (calendarRawItem.LectureId != -1)
                {
                    lesson.Lecturer = _lecturers.FirstOrDefault(a => a.ExternalId == calendarRawItem.LectureId);
                }
                //Set classroom
                if (calendarRawItem.ClassroomId != -1)
                {
                    lesson.Classroom = _classrooms.FirstOrDefault(a => a.ExternalId == calendarRawItem.ClassroomId);
                }
                //Set LessonCount
                if (calendarRawItem.LessonCount != -1)
                {
                    lesson.LessonCount = calendarRawItem.LessonCount;
                }
                //Set LessonNumber
                if (calendarRawItem.LessonNumber != -1)
                {
                    lesson.LessonNumber = calendarRawItem.LessonNumber;
                }
                //Set Fraction
                if (calendarRawItem.Fraction != -1)
                {
                    lesson.Fraction = (Fraction) calendarRawItem.Fraction;
                }
                //Set Subgroup
                if (calendarRawItem.SubGroup != -1)
                {
                    lesson.SubGroup = (SubGroup) calendarRawItem.SubGroup;
                }
                _lessons.Add(lesson);
            }
        }

        public async Task<List<Lesson>> GetLessonsOnDate(DateTime date, int groupId)
        {
            if (_lessons == null) await Task.Run(() => CreateLessonsList());

            var startPoint = date.AddDays(-56);
            List<Lesson> lessonList = new List<Lesson>();

            while (startPoint <= date)
            {
                var tempList = _lessons.Where(lesson => lesson.LessonDate == startPoint && lesson.Group.ExternalId == groupId);
                if (tempList.Any())
                {
                    lessonList.Clear();
                    foreach (var lesson in tempList) //TODO: to linq
                    {
                        lessonList.Add(lesson);
                    }
                }
                    
                startPoint = startPoint.AddDays(7);

            }

            return lessonList;
        }

    }

    public class RawStringParser
    {
        public List<CalendarRawItem> ConvertCalendarRaw(List<CalendarRawItem> collection, string rawString)
        {
            for (int i = 13; i < rawString.Length; i++)
            {
                if (rawString[i] == '[')
                {
                    if(rawString[i+1] == '[') continue;
                    //inside [ ]
                    i++;
                    int offset = 1; //TODO: safe to remove

                    CalendarRawItem item = new CalendarRawItem();
                    StringBuilder currentText = new StringBuilder();
                    List<string> valuesInBrackets = new List<string>();

                    while (rawString[i] != ']')
                    {
                        while (rawString[i] == '"') i++;
                        if(rawString[i] == ']') continue;
                        while (rawString[i] != '"' && rawString[i] != ',')
                        {
                            currentText.Append(rawString[i]);
                            i++;
                            offset++; 
                        }

//                        if (!String.IsNullOrEmpty(currentText.ToString()))
//                        {
                            valuesInBrackets.Add(currentText.ToString());
                            currentText.Clear();
//                        }

                        if (rawString[i] == '"')
                        {
                            i++;
                            offset++;
                        }

                        if (rawString[i] == ',')
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
                        item.SubjectName = Decode.Unescape(valuesInBrackets[1]);
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

        public List<Group> ConvertGroupRaw(List<Group> collection, string rawString)
        {
            for (int i = 13; i < rawString.Length; i++)
            {
                if (rawString[i] == '[')
                {
                    if (rawString[i + 1] == '[') continue;
                    //inside [ ]
                    i++;
                    int offset = 1; //TODO: safe to remove

                    Group item = new Group();
                    StringBuilder currentText = new StringBuilder();
                    List<string> valuesInBrackets = new List<string>();

                    while (rawString[i] != ']')
                    {
                        while (rawString[i] == '"') i++;
                        if (rawString[i] == ']') continue;
                        while (rawString[i] != '"' && rawString[i] != ',')
                        {
                            currentText.Append(rawString[i]);
                            i++;
                            offset++;
                        }

                        valuesInBrackets.Add(currentText.ToString());
                        currentText.Clear();

                        if (rawString[i] == '"')
                        {
                            i++;
                            offset++;
                        }

                        if (rawString[i] == ',')
                        {
                            i++;
                            offset++;
                        }
                    }

                    if(valuesInBrackets.Count == 0) continue;

                    item.ExternalId = int.Parse(valuesInBrackets[0]);
                    item.ShortName = Decode.Unescape(valuesInBrackets[1]);

                    collection.Add(item);

                }
            }

            return collection;
        }

        public List<Lecturer> ConvertLecturersRaw(List<Lecturer> collection, string rawString)
        {
            for (int i = 13; i < rawString.Length; i++)
            {
                if (rawString[i] == '[')
                {
                    if (rawString[i + 1] == '[') continue;
                    //inside [ ]
                    i++;
                    int offset = 1; //TODO: safe to remove

                    Lecturer item = new Lecturer();
                    StringBuilder currentText = new StringBuilder();
                    List<string> valuesInBrackets = new List<string>();

                    while (rawString[i] != ']')
                    {
                        while (rawString[i] == '"') i++;
                        if (rawString[i] == ']') continue;
                        while (rawString[i] != '"' && rawString[i] != ',')
                        {
                            currentText.Append(rawString[i]);
                            i++;
                            offset++;
                        }

                        valuesInBrackets.Add(currentText.ToString());
                        currentText.Clear();

                        if (rawString[i] == '"')
                        {
                            i++;
                            offset++;
                        }

                        if (rawString[i] == ',')
                        {
                            i++;
                            offset++;
                        }
                    }

                    item.ExternalId = int.Parse(valuesInBrackets[0]);
                    item.FullName = Decode.Unescape(valuesInBrackets[1]);

                    collection.Add(item);

                }
            }
            return collection;
        }

        public List<Classroom> ConvertClassroomsRaw(List<Classroom> collection, string rawString)
        {
            for (int i = 13; i < rawString.Length; i++)
            {
                if (rawString[i] == '[')
                {
                    if (rawString[i + 1] == '[') continue;
                    //inside [ ]
                    i++;
                    int offset = 1; //TODO: safe to remove

                    Classroom item = new Classroom();
                    StringBuilder currentText = new StringBuilder();
                    List<string> valuesInBrackets = new List<string>();

                    while (rawString[i] != ']')
                    {
                        while (rawString[i] == '"') i++;
                        if (rawString[i] == ']') continue;
                        while (rawString[i] != '"' && rawString[i] != ',')
                        {
                            currentText.Append(rawString[i]);
                            i++;
                            offset++;
                        }

                        valuesInBrackets.Add(currentText.ToString());
                        currentText.Clear();

                        if (rawString[i] == '"')
                        {
                            i++;
                            offset++;
                        }

                        if (rawString[i] == ',')
                        {
                            i++;
                            offset++;
                        }
                    }

                    item.ExternalId = int.Parse(valuesInBrackets[0]);
                    item.Name = Decode.Unescape(valuesInBrackets[1]);

                    collection.Add(item);

                }
            }
            return collection;
        }
    }
}
