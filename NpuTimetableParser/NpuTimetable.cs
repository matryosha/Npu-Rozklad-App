using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Decode = System.Text.RegularExpressions.Regex;

namespace NpuTimetableParser
{
    public class NpuParser
    {
        private readonly IRestClient _client;
        private readonly RawStringParser _rawParser = new RawStringParser();
        private readonly NpuParserHelper _helper;
        private readonly List<Classroom> _classrooms;
        private readonly List<Lesson> _lessons = new List<Lesson>();
        private List<Group> _groups;
        private List<Lecturer> _lecturers;
        private List<CalendarRawItem> _calendarRawList;
        private List<Faculty> _faculties;
        private int _deltaGapInDays = -140;
        private string _siteUrl = "http://ei.npu.edu.ua";

        public NpuParser(IRestClient client, string faculty = "fi")
        {
            _client = client;
            _helper = new NpuParserHelper(_client, _rawParser, faculty); //TODO IoC
        }

        public NpuParser(string faculty)
        {
            _client = new RestClient(_siteUrl);//TODO: extract string
            _helper = new NpuParserHelper(_client, _rawParser,faculty);
        }

        /// <summary>
        /// Change interval from which lessons search starts parsing.
        /// Must be a multiple of 7
        /// Default is -140
        /// </summary>
        /// <param name="days"></param>
        public void SetDeltaIntervalInDays(int days)
        {
            if (days>=0) throw new Exception("Delta must be negative"); 
            if (days % 7 != 0) throw new Exception("Delta must be multiple of 7");
            _deltaGapInDays = days;
        }

        public List<Lesson> GetAllLessons()
        {
            return new List<Lesson>(_lessons);
        }

        public void SetSiteUrl(string url)
        {
            _siteUrl = url;
        }

        public async Task<List<Faculty>> GetFaculties()
        {
            return await Task.Run(() => _faculties ?? (_faculties = _helper.GetFaculties()));
        }

        public async Task<List<Lesson>> GetLessonsOnDate(DateTime date, int groupId)
        {
            if (_lessons == null || _lessons.Count == 0)
                await Task.Run(() =>
                    _helper.CreateLessonsList(_calendarRawList,_groups,_lecturers,_classrooms,_lessons));

            var startPoint = date.AddDays(_deltaGapInDays);
            List<Lesson> resultLessonsList = new List<Lesson>();

            while (startPoint <= date)
            {
                IEnumerable<Lesson> moreRecentLessonsList = new List<Lesson>();

                moreRecentLessonsList = _lessons.Where(lesson => lesson.Group != null &&
                                                                 lesson.Group.ExternalId == groupId &&
                                                                 lesson.LessonDate == startPoint);

                if (moreRecentLessonsList.Any())
                {
                    //Doing merging only if current lessonList isn't empty
                    if (resultLessonsList.Any())
                        NpuParserHelper.MergeLessonsList(resultLessonsList, moreRecentLessonsList);
                    else
                        foreach (var lesson in moreRecentLessonsList) //TODO: to linq
                            resultLessonsList.Add(lesson);
                }
                    
                startPoint = startPoint.AddDays(7);

            }

            var deleteOldLessons = new List<Lesson>(resultLessonsList);

            foreach (var lesson in resultLessonsList)
            {
                int deltaDateTime;
                if (lesson.Fraction == Fraction.None)
                    deltaDateTime = (date - lesson.LessonDate).Days / 7;
                else
                {
                    deltaDateTime = ((date - lesson.LessonDate).Days / 7) / 2; //There is might be a problem when number is odd
                }

                if (lesson.LessonCount - deltaDateTime <= 0) deleteOldLessons.Remove(lesson);

            }
         
            return deleteOldLessons;
        }
    }

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
                    item.SubjectName = Decode.Unescape(rawValue[1]);
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
                    ShortName = Decode.Unescape(rawValue[1])
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
                    FullName = Decode.Unescape(rawValue[1])
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
                    Name = Decode.Unescape(rawValue[1])
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
                    ShortName = Decode.Unescape(rawValue[0]),
                    FullName = Decode.Unescape(rawValue[1])
                };
                result.Add(item);
            }

            return result;
        }

        private static IEnumerable<List<string>> GetValues(string rawString)
        {
            var values = new List<List<string>>();
            for (int i = 13; i < rawString.Length; i++)
            {
                if (rawString[i] == '[')
                {
                    if (rawString[i + 1] == '[') continue;
                    //inside [ ]
                    i++;

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
                        }

                        valuesInBrackets.Add(currentText.ToString());
                        currentText.Clear();

                        if (rawString[i] == '"') i++;

                        if (rawString[i] == ',') i++;
                    }

                    values.Add(valuesInBrackets);
                }
            }
            return values;
        }
    }

    public class NpuParserHelper
    {
        private IRestClient _client;
        private RawStringParser _rawParser;
        private string _faculty;

        public NpuParserHelper(IRestClient client, RawStringParser rawParser, string faculty)
        {
            _client = client;
            _rawParser = rawParser;
            _faculty = faculty;
        }

        public void CreateLessonsList(List<CalendarRawItem> calendarRawList, List<Group> groups, 
            List<Lecturer> lecturers, List<Classroom> classrooms, List<Lesson> lessons)
        {
            calendarRawList = FillCalendarRawList();
            groups = FillGroupList();
            lecturers = FillLecturersList();
            classrooms = FillClassroomsList();

            foreach (var calendarRawItem in calendarRawList)
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
                    lesson.Group = groups.FirstOrDefault(a => a.ExternalId == calendarRawItem.GroupId);
                }
                //Set lecture
                if (calendarRawItem.LectureId != -1)
                {
                    lesson.Lecturer = lecturers.FirstOrDefault(a => a.ExternalId == calendarRawItem.LectureId);
                }
                //Set classroom
                if (calendarRawItem.ClassroomId != -1)
                {
                    lesson.Classroom = classrooms.FirstOrDefault(a => a.ExternalId == calendarRawItem.ClassroomId);
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
                    lesson.Fraction = (Fraction)calendarRawItem.Fraction;
                }
                //Set Subgroup
                if (calendarRawItem.SubGroup != -1)
                {
                    lesson.SubGroup = (SubGroup)calendarRawItem.SubGroup;
                }
                lessons.Add(lesson);
            }
        }

        public string SiteRequest(string code, string faculty)
        {
            if (_client == null) throw new Exception("client is null");

            var request = new RestRequest("Server.php", Method.POST);

            request.AddParameter("code", code);
            request.AddParameter("params", "");
            request.AddParameter("loginpass", "");
            request.AddParameter("faculty", faculty);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");

            IRestResponse response = _client.Execute(request);
            return response.Content;
        }

        public List<CalendarRawItem> FillCalendarRawList()
        {
            var calendarRawList = new List<CalendarRawItem>();

            var clientResponse = SiteRequest("get calendar", _faculty);
            calendarRawList = _rawParser.DeserializeCalendar(clientResponse);

            return calendarRawList;
        }
     
        public List<Group> FillGroupList()
        {
            var groupList = new List<Group>();

            var clientResponse = SiteRequest("get groups", _faculty);
            groupList = _rawParser.DeserializeGroups(clientResponse);

            return groupList;
        }

        public List<Lecturer> FillLecturersList()
        {
            var lecturesList = new List<Lecturer>();
;
            var clientResponse = SiteRequest("get lectors", _faculty);
            lecturesList = _rawParser.DeserializeLecturers(clientResponse);

            return lecturesList;
        }

        public List<Classroom> FillClassroomsList()
        {
            var classroomsList = new List<Classroom>();

            var clientResponse = SiteRequest("get auditories", _faculty);
            classroomsList = _rawParser.DeserializeClassrooms(clientResponse);

            return classroomsList;
        }

        public List<Faculty> GetFaculties()
        {
            return _rawParser.DeserializeFaculties(SiteRequest("get faculties", "fi"));
        }

        /// <summary>
        /// Resolve all conflicts and put new lesson in right place
        /// </summary>
        /// <param name="resultLessonsList"></param>
        /// <param name="moreRecentLessonsList"></param>
        public static void MergeLessonsList(List<Lesson> resultLessonsList, IEnumerable<Lesson> moreRecentLessonsList)
        {
            foreach (var newLesson in moreRecentLessonsList)
            {
                //Check if there is a lesson with the same lesson number
                var sameLessonsNumber = resultLessonsList.Where(l => l.LessonNumber == newLesson.LessonNumber).ToList();
                if (!sameLessonsNumber.Any())
                {
                    resultLessonsList.Add(newLesson);
                    continue;
                }
                foreach (var oldLessonWithSameNumber in sameLessonsNumber)
                {
                    if (resultLessonsList.Contains(newLesson)) continue;
                    if (newLesson.Fraction == Fraction.None &&
                        newLesson.SubGroup == SubGroup.None)
                    {
                        //Remove all lessons with that lesson number
                        resultLessonsList.RemoveAll(l => l.LessonNumber == newLesson.LessonNumber);
                        resultLessonsList.Add(newLesson);
                        continue;
                    }
                    if (oldLessonWithSameNumber.Fraction == newLesson.Fraction &&
                        newLesson.SubGroup == SubGroup.None)
                    {
                        resultLessonsList.RemoveAll(l => l.LessonNumber == newLesson.LessonNumber &&
                                                         l.Fraction == newLesson.Fraction);
                        resultLessonsList.Add(newLesson);
                        continue;
                    }
                    if (oldLessonWithSameNumber.Fraction == newLesson.Fraction &&
                        oldLessonWithSameNumber.SubGroup == newLesson.SubGroup &&
                        oldLessonWithSameNumber.SubGroup != SubGroup.None)
                    {
                        ReplaceLesson(resultLessonsList, newLesson, oldLessonWithSameNumber);
                        continue;
                    }
                    if (oldLessonWithSameNumber.Fraction == newLesson.Fraction &&
                        oldLessonWithSameNumber.SubGroup == newLesson.SubGroup)
                    {
                        ReplaceLesson(resultLessonsList, newLesson, oldLessonWithSameNumber);
                        continue;
                    }
                    resultLessonsList.Add(newLesson);
                }
            }
        }
        public static void ReplaceLesson(List<Lesson> list, Lesson newLesson, Lesson oldLesson)
        {
            list.Remove(oldLesson);
            list.Add(newLesson);
        }

    }
}
