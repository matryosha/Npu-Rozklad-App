using System;
using System.Threading.Tasks;

namespace NpuRozklad.LessonsProvider
{
    public interface IOddDayWeekChecker
    {
        Task<bool> IsOddDayWeek(DateTime dateTime);
    }
}