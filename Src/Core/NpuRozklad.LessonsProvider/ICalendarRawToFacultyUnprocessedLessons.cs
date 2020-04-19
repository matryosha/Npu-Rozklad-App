using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.LessonsProvider.Entities;

namespace NpuRozklad.LessonsProvider
{
    public interface ICalendarRawToFacultyUnprocessedLessons
    {
        Task<ICollection<ExtendedLesson>> Transform(ICollection<CalendarRawItem> calendarRawList, Faculty faculty);
    }
}