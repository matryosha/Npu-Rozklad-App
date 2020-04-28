using System;

namespace NpuRozklad.Core.Interfaces
{
    /// <summary>
    /// Converter for simple day of week navigation
    /// </summary>
    public interface IDayOfWeekToDateTime
    {
        /// <summary>
        /// Converts <see cref="System.DayOfWeek"/> to local <see cref="System.DateTime"/>
        /// Example: If current date is April 21 and it's tuesday.
        /// 
        /// Then when <paramref name="dayOfWeek"/> is <see cref="DayOfWeek.Monday"/> and
        /// <paramref name="asNextWeekDate"/> is false then returned value would be April 20 Monday
        ///
        /// When <paramref name="dayOfWeek"/> is <see cref="DayOfWeek.Tuesday"/> and
        /// <paramref name="asNextWeekDate"/> is True then returned value would be April 28 Tuesday
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="asNextWeekDate">When true returned datetime will be for next week</param>
        /// <returns>Relative to current day <see cref="System.DateTime"/> for which <see cref="System.DayOfWeek"/>
        /// is <paramref name="dayOfWeek"/></returns>
        DateTime GetDate(DayOfWeek dayOfWeek, bool asNextWeekDate = false);
    }
}