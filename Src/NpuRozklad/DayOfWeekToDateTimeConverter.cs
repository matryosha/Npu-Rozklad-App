using System;
using NpuRozklad.Core.Infrastructure;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad
{
    public class DayOfWeekToDateTimeConverter : IDayOfWeekToDateTimeConverter
    {
        private readonly ILocalDateService _localDateService;
        private const DayOfWeek FirstDayOfWeek = DayOfWeek.Monday;

        public DayOfWeekToDateTimeConverter(ILocalDateService localDateService)
        {
            _localDateService = localDateService;
        }
        public DateTime Convert(DayOfWeek dayOfWeek, bool asNextWeekDate = false)
        {
            var currentDay = _localDateService.LocalDateTime;
            var firstDayOfCurrentWeek = GetFirstDayOfWeek(currentDay);

            var resultDate = firstDayOfCurrentWeek.AddDays(NpuDateTimeHelper.DayOfWeekToLocal(dayOfWeek));
            if (asNextWeekDate) resultDate = resultDate.AddDays(7);

            return resultDate;
        }


        
        // From https://github.com/FluentDateTime/FluentDateTime/blob/dba3f481fda3b9ee3c1b5ed717999bdad532c38e/src/FluentDateTime/DateTime/DateTimeExtensions.cs#L659
        public static DateTime GetFirstDayOfWeek(DateTime dateTime)
        {
            var offset = dateTime.DayOfWeek - FirstDayOfWeek < 0 ? 7 : 0;
            var numberOfDaysSinceBeginningOfTheWeek = dateTime.DayOfWeek + offset - FirstDayOfWeek;

            return dateTime.AddDays(-numberOfDaysSinceBeginningOfTheWeek);
        }
    }
}