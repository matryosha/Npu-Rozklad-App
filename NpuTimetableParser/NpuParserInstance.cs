using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace NpuTimetableParser
{
    public class NpuParserInstance
    {
        private readonly IRestClient _client;
        private readonly RawStringParser _rawParser;
        private readonly NpuParserHelper _helper;
        private readonly List<Classroom> _classrooms;
        private readonly List<Lesson> _lessons = new List<Lesson>();
        private List<Group> _groups;
        private List<Lecturer> _lecturers;
        private List<CalendarRawItem> _calendarRawList;
        private List<Faculty> _faculties;
        private DateTime _lastLessonUpdateTime;
        private int _deltaGapInDays = -140;
        private string _faculty;
        private string _siteUrl = "http://ei.npu.edu.ua";

        public NpuParserInstance(IRestClient client, RawStringParser rawParser, string faculty = "fi")
        {
            _client = client;
            _rawParser = rawParser;
            _helper = new NpuParserHelper(_client, _rawParser, faculty); //TODO IoC
            _faculty = faculty;
        }

        public NpuParserInstance(string faculty, RawStringParser rawParser)
        {
            _client = new RestClient(_siteUrl);
            _faculty = faculty;
            _rawParser = rawParser;
            _helper = new NpuParserHelper(_client, _rawParser,_faculty);
        }

        public NpuParserInstance(RawStringParser rawParser)
        {
            _rawParser = rawParser;
            _client = new RestClient(_siteUrl);
            _helper = new NpuParserHelper(_client, _rawParser);
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

        public string GetCurrentFaculty()
        {
            return _faculty;
        }

        public void SetSiteUrl(string url)
        {
            _siteUrl = url;
        }

        public void SetFaculty(string faculty)
        {
            if (_faculties != null)
            {
                if(!_faculties.Any(f => f.ShortName == faculty)) throw new Exception($"There is not such a faculty: {faculty}");
            }

            _faculty = faculty;
            _helper.SetFaculty(faculty);
        }

        public void SetFaculty(Faculty faculty)
        {
            if (_faculties != null)
            {
                if (!_faculties.Any(f => f.ShortName == faculty.ShortName)) throw new Exception($"There is not such a faculty: {faculty}");
            }

            _faculty = faculty.ShortName;
            _helper.SetFaculty(faculty.ShortName);
        }

        public async Task<List<Faculty>> GetFaculties()
        {
            return await Task.Run(() => _faculties ?? (_faculties = _helper.GetFaculties()));
        }

        public async Task<List<Group>> GetGroups()
        {
            return await Task.Run(() => _groups ?? (_groups = _helper.GetGroups(_faculty))); 
        }

        public async Task<List<Lesson>> GetLessonsOnDate(DateTime date, int groupId)
        {
            if (_lessons.Count == 0 ||
                DateTime.Now - _lastLessonUpdateTime > TimeSpan.FromMinutes(1))
            {
                //Do not clear before getting new lessons
                _lessons.Clear();
                await Task.Run(() =>
                    _helper.CreateLessonsList(_calendarRawList, _groups, _lecturers, _classrooms, _lessons));
                _lastLessonUpdateTime = DateTime.Now;
            }

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
            var currentWeekInt = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday) % 2;
            var currentWeek = Fraction.None;
            if (currentWeekInt == 1)
            {
                currentWeek = Fraction.Numerator;
            }
            else
            {
                currentWeek = Fraction.Denominator;
            }

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

            if (currentWeek == Fraction.Numerator)
            {
                return deleteOldLessons.Where(l => l.Fraction == Fraction.None || l.Fraction == currentWeek)
                    .OrderBy(l => l.LessonNumber).ToList();
            } else if (currentWeek == Fraction.Denominator)
            {
                return deleteOldLessons.Where(l => l.Fraction == Fraction.None || l.Fraction == currentWeek)
                    .OrderBy(l => l.LessonNumber).ToList();
            }


            return deleteOldLessons;
        }
    }
}
