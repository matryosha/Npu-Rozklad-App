using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.LessonsProvider.Entities;

namespace NpuRozklad.LessonsProvider.Holders.Interfaces
{
    internal interface ICalendarRawItemHolder
    {
        Task<ICollection<CalendarRawItem>> GetCalendarItems();
    }
}