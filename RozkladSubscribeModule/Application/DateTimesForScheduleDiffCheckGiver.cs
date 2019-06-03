using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RozkladNpuBot.Application.Helpers;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Infrastructure;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Application
{
    internal class DateTimesForScheduleDiffCheckGiver :
        IDateTimesForScheduleDiffCheckGiver
    {
        private readonly ILogger<DateTimesForScheduleDiffCheckGiver> _logger;
        private readonly CheckTimeType _checkType;


        public DateTimesForScheduleDiffCheckGiver(
            IOptions<RozkladSubscribeServiceOptions> options,
            ILogger<DateTimesForScheduleDiffCheckGiver> logger)
        {
            _logger = logger;
            _checkType = options.Value.CheckTimeType;
        }

        public List<DateTime> GetDates()
        {
            switch (_checkType)
            {
                case CheckTimeType.OnlyNextDay:
                    return OnlyNextDayDate();
                case CheckTimeType.OnlyTwoNextDays:
                    return OnlyTwoNextDaysDates();
                case CheckTimeType.LastDaysOfCurrentWeek:
                    return LastDaysOfCurrentWeekDates();
                case CheckTimeType.LastDaysOfCurrentWeekAndNextWeek:
                    return LastDaysOfCurrentWeekAndNextWeekDates();
                default:
                {
                    _logger.LogWarning(
                        $"Unknown type ${nameof(_checkType)}. Return OnlyNextDayDate");
                    return OnlyNextDayDate();
                }
            }
        }

        private List<DateTime> LastDaysOfCurrentWeekAndNextWeekDates()
        {
            throw new NotImplementedException();
        }

        private List<DateTime> LastDaysOfCurrentWeekDates()
        {
            var today = DateTime.Today.ToLocal();
            var resultDates = new List<DateTime>();
            if (today.DayOfWeek == DayOfWeek.Saturday ||
                today.DayOfWeek == DayOfWeek.Sunday)
            {
                return resultDates;
            }

            if (today.DayOfWeek == DayOfWeek.Friday)
            {
                if (today.Hour < 9)
                {
                    resultDates.Add(today);
                    return resultDates;
                }

                return resultDates;
            }

            if (today.Hour < 9)
            {
                resultDates.Add(today);
            }

            for (int i = 1; i < (int)(DayOfWeek.Saturday - today.DayOfWeek); i++)
            {
                resultDates.Add(today.AddDays(i));
            }

            return resultDates;
        }

        private List<DateTime> OnlyTwoNextDaysDates()
        {
            throw new NotImplementedException();
        }

        private List<DateTime> OnlyNextDayDate()
        {
            var today = DateTime.Today.ToLocal();
            var result = new List<DateTime>();
            if (today.DayOfWeek == DayOfWeek.Saturday)
            {
                result.Add(today.AddDays(2));
                return result;
            }

            if (today.DayOfWeek == DayOfWeek.Friday)
            {
                result.Add(today.AddDays(3));
                return result;
            }

            result.Add(today.AddDays(1));
            return result;
        }
    }
}