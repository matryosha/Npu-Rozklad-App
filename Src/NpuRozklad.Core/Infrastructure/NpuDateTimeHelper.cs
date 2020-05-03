using System;

namespace NpuRozklad.Core.Infrastructure
{
    public static class NpuDateTimeHelper
    {
        /// <summary>
        /// Helps simplify some loops
        /// </summary>
        /// <param name="dayOfWeekNumber"></param>
        /// <returns>
        /// <para>0 to <see cref="DayOfWeek.Monday"/></para> 
        /// <para>1 to <see cref="DayOfWeek.Tuesday"/></para>  
        /// <para>2 to <see cref="DayOfWeek.Wednesday"/></para>  
        /// <para>3 to <see cref="DayOfWeek.Thursday"/></para>  
        /// <para>4 to <see cref="DayOfWeek.Friday"/></para>  
        /// <para>5 to <see cref="DayOfWeek.Saturday"/></para>  
        /// <para>6 to <see cref="DayOfWeek.Sunday"/></para>  
        /// </returns>
        public static DayOfWeek DayOfWeekNumberToLocalDayOfWeek(int dayOfWeekNumber)
        {
            return dayOfWeekNumber == 6
                ? DayOfWeek.Sunday
                : (DayOfWeek) (dayOfWeekNumber + 1);
        }
        
        public static int DayOfWeekToLocal(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday
                ? 6
                : (int) dayOfWeek - 1;
        }
    }
}