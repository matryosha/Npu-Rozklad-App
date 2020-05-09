using System;
using System.Threading.Tasks;

namespace NpuRozklad.LessonsProvider
{
    internal interface IOddDayWeekChecker
    {
        Task<bool> IsOddDayWeek(DateTime dateTime);
    }
}