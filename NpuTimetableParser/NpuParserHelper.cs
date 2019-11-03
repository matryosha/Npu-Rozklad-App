using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using RestSharp;
using RozkladNpuBot.Common;

namespace NpuTimetableParser
{
    public class NpuParserHelper
    {
        private IRestClient _client;
        private RawStringParser _rawParser;
        private string _faculty;
        private List<Faculty> _faculties;

        public NpuParserHelper(IRestClient client, RawStringParser rawParser, string faculty)
        {
            _client = client;
            _rawParser = rawParser;
            _faculty = faculty;
        }

        public NpuParserHelper(IRestClient client, RawStringParser rawParser)
        {
            _client = client;
            _rawParser = rawParser;
        }

        public void CreateLessonsList(List<CalendarRawItem> calendarRawList, List<Group> groups,
            List<Lecturer> lecturers, List<Classroom> classrooms, List<Lesson> lessons)
        {
            if (string.IsNullOrEmpty(_faculty)) throw new Exception("Faculty hasn't been set");
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
            if (response.ContentLength == 0) throw new HttpRequestException("Response content was empty", response.ErrorException);
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
            groupList = _rawParser.DeserializeGroups(clientResponse, _faculty);

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

        public List<Faculty>  GetFaculties()
        {
            return _faculties ?? (_faculties = 
                       _rawParser.DeserializeFaculties(
                           SiteRequest("get faculties", "fi")));
        }

        public List<Group> GetGroups(string faculty)
        {
            return _rawParser.DeserializeGroups(
                SiteRequest("get groups", faculty),
                _faculty);
        }

        public bool IsOddDayWeek(DateTime date)
        {
            var clientResponse = SiteRequest("get settings", "fi");
            // alg from CalendarPreparator.js:452
            var (oddEvenDay, isOddDay) = _rawParser.GetSettings(clientResponse);
            var startWeekDate = GetStartWeekDate(date.ToLocal());

            var distanceToOddEvenDay = (oddEvenDay - startWeekDate).Days;
            if ((distanceToOddEvenDay % 14 == 0))
                return isOddDay;

            return !isOddDay;
        }

        private DateTime GetStartWeekDate(DateTime date)
        {
            // rozlad client code adapted to c#
            // DynamicOIL.js:141
            // 1 IQ moves
            if (date.DayOfWeek ==  DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(7);
            }
	
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
        
        public void SetFaculty(string faculty)
        {
            _faculty = faculty;
        }

        public void SetClient(IRestClient client)
        {
            _client = client;
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
