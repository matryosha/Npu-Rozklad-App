using System;
using System.Globalization;

namespace NpuRozklad.Core.Interfaces
{
    /// <summary>
    /// Converter for simple day of week navigation
    /// </summary>
    public interface IDayOfWeekToDateTimeConverter
    {
        /// <summary>
        /// Converts <see cref="System.DayOfWeek"/> to local <see cref="System.DateTime"/>
        /// <para>Example: If current date is April 21 and it's tuesday then:</para>
        /// 
        /// <para>
        /// When <paramref name="dayOfWeek"/> is <see cref="DayOfWeek.Monday"/> and
        /// <paramref name="asNextWeekDate"/> is <c>false</c> then returned value would be April 20 Monday.
        /// </para>
        /// 
        /// <para>
        /// When <paramref name="dayOfWeek"/> is <see cref="DayOfWeek.Tuesday"/> and
        /// <paramref name="asNextWeekDate"/> is <c>true</c> then returned value would be April 28 Tuesday.
        /// </para>
        ///
        /// <para>
        /// Note that method considers <see cref="DateTimeFormatInfo.FirstDayOfWeek"/> as <see cref="DayOfWeek.Monday"/>
        /// </para>
        /// </summary>
        /// <param name="dayOfWeek"><see cref="System.DayOfWeek"/> value to get the date from</param>
        /// <param name="asNextWeekDate">When true returned datetime will be for next week</param>
        /// <returns>Relative to current day <see cref="System.DateTime"/> for which <see cref="System.DayOfWeek"/>
        /// is <paramref name="dayOfWeek"/></returns>
        DateTime Convert(DayOfWeek dayOfWeek, bool asNextWeekDate = false);
    }
}