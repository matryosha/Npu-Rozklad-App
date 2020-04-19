using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.LessonsProvider.Entities;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider
{
    public class UnprocessedExtendedLessonsManager : 
        PeriodicOperationExecutor,
        IUnprocessedExtendedLessonsHolder, ICalendarRawToFacultyUnprocessedLessons
    {
        private readonly IGroupsHolder _groupsHolder;
        private readonly ILecturersHolder _lecturersHolder;
        private readonly IClassroomsHolder _classroomsHolder;
        private readonly ICalendarRawItemHolder _calendarRawItemHolder;

        private readonly Dictionary<Faculty, ICollection<ExtendedLesson>> _unprocessedLessonsCache = 
            new Dictionary<Faculty, ICollection<ExtendedLesson>>();
        private readonly ConcurrentDictionary<Faculty, OneManyLock> _facultyLocks =
            new ConcurrentDictionary<Faculty, OneManyLock>();
        private readonly OneManyLock _cacheLock = new OneManyLock();
        // to options
        private int _cacheLifeTimeInMinutes = 10;
        
        public UnprocessedExtendedLessonsManager(IGroupsHolder groupsHolder, ILecturersHolder lecturersHolder,
            IClassroomsHolder classroomsHolder, ICalendarRawItemHolder calendarRawItemHolder)
        {
            _groupsHolder = groupsHolder;
            _lecturersHolder = lecturersHolder;
            _classroomsHolder = classroomsHolder;
            _calendarRawItemHolder = calendarRawItemHolder;

            PeriodicCallIntervalInSeconds = _cacheLifeTimeInMinutes * 60;
            PeriodicAction = ClearCache;
        }
        
        public async Task<ICollection<ExtendedLesson>> GetFacultyUnprocessedLessons(Faculty faculty)
        {
            
            // check if key is present
            // if not populate cache
            // return value
            ICollection<ExtendedLesson> result;
            
            var facultyLock = _facultyLocks.GetOrAdd(faculty, new OneManyLock());
            facultyLock.Enter(false);
            _cacheLock.Enter(false);

            if(!_unprocessedLessonsCache.ContainsKey(faculty))
            {
                facultyLock.Leave();
                _cacheLock.Leave();
                facultyLock.Enter(true);
                _cacheLock.Enter(false);
                
                if (!_unprocessedLessonsCache.ContainsKey(faculty))
                {
                    _cacheLock.Leave();
                    
                    var facultyRawLessons = await GetFacultyRawLessonsInternal(faculty);

                    _cacheLock.Enter(true);
                    _unprocessedLessonsCache.Add(faculty, facultyRawLessons);
                }
            }

            result = new List<ExtendedLesson>(_unprocessedLessonsCache[faculty]);
            _cacheLock.Leave();
            facultyLock.Leave();
            
            return result;
        }

        private async Task<ICollection<ExtendedLesson>> GetFacultyRawLessonsInternal(Faculty faculty)
        {
            var calendarRawItems =
                await _calendarRawItemHolder.GetCalendarItems()
                    .ConfigureAwait(false);

            var facultyRawLessons =
                await Transform(calendarRawItems, faculty)
                    .ConfigureAwait(false);
            return facultyRawLessons;
        }


        /*
         * Some legacy
         */
        
        public async Task<ICollection<ExtendedLesson>> Transform(ICollection<CalendarRawItem> calendarRawList, Faculty faculty)
        {
            var resultList = new List<ExtendedLesson>(calendarRawList.Count);
            foreach (var calendarRawItem in calendarRawList)
            {
                var lesson = new ExtendedLesson();
                //Set lesson date
                lesson.LessonSetUpDate = calendarRawItem.LessonSetDate;
                //Set subject
                if (!string.IsNullOrEmpty(calendarRawItem.SubjectName))
                {
                    var subject = new Subject(calendarRawItem.SubjectName);
                    lesson.Subject = subject;
                }

                //Set group
                lesson.Group = (await _groupsHolder.GetFacultiesGroups().ConfigureAwait(false))
                    .FirstOrDefault(a => a.TypeId == calendarRawItem.GroupId);

                //Set lecture
                lesson.Lecturer = (await _lecturersHolder.GetLecturers(faculty).ConfigureAwait(false))
                    .FirstOrDefault(a => a.TypeId == calendarRawItem.LecturerId);

                //Set classroom
                lesson.Classroom = (await _classroomsHolder.GetClassrooms(faculty).ConfigureAwait(false))
                    .FirstOrDefault(a => a.TypeId == calendarRawItem.ClassroomId);

                //Set LessonCount
                lesson.LessonCount = Convert.ToInt32(calendarRawItem.LessonCount);

                //Set LessonNumber
                lesson.LessonNumber = Convert.ToInt32(calendarRawItem.LessonNumber);

                //Set Fraction
                lesson.Fraction = (Fraction) calendarRawItem.Fraction;

                //Set Subgroup
                lesson.SubGroup = (SubGroup) calendarRawItem.SubGroup;
                resultList.Add(lesson);
            }

            return resultList;
        }
        
        private Task ClearCache()
        {
            _cacheLock.Enter(true);
            _unprocessedLessonsCache.Clear();
            _cacheLock.Leave();
            
            return Task.CompletedTask;
        }
    }
}