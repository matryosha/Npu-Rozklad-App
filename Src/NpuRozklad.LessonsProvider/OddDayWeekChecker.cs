using System;
using System.Threading.Tasks;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider
{
    internal class OddDayWeekChecker : IOddDayWeekChecker
    {
        private readonly ISettingsHolder _settingsHolder;

        public OddDayWeekChecker(ISettingsHolder settingsHolder)
        {
            _settingsHolder = settingsHolder;
        }
        
        public async Task<bool> IsOddDayWeek(DateTime dateTime)
        {
            // alg from CalendarPreparator.js:452
            var (oddEvenDay, isOddDay) = await _settingsHolder.GetSettings().ConfigureAwait(false);
            var startWeekDate = GetStartWeekDate(dateTime);

            var distanceToOddEvenDay = (oddEvenDay - startWeekDate).Days;
            if (distanceToOddEvenDay % 14 == 0)
                return isOddDay;

            return !isOddDay;
        }

        private static DateTime GetStartWeekDate(DateTime date)
        {
            // rozklad client code adapted to c#
            // DynamicOIL.js:141
            // 1 IQ moves
            if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(7);
            }

            var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}