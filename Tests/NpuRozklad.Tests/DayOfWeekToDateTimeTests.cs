using System;
using System.Collections.Generic;
using Moq;
using NpuRozklad.Core.Interfaces;
using NUnit.Framework;

namespace NpuRozklad.Tests
{
    public class DayOfWeekToDateTimeTests
    {
        public static ICollection<DateTime> CurrentWeekExpectedDates { get; set; } = new List<DateTime>
        {
            new DateTime(2020, 04,26 ),
            new DateTime(2020, 04,20 ),
            new DateTime(2020, 04,21 ),
            new DateTime(2020, 04,22 ),
            new DateTime(2020, 04,23 ),
            new DateTime(2020, 04,24 ),
            new DateTime(2020, 04,25 ),
        };
        
        public static ICollection<DateTime> NextWeekExpectedDates { get; set; } = new List<DateTime>
        {
            new DateTime(2020, 05,3 ),
            new DateTime(2020, 4,27 ),
            new DateTime(2020, 4,28),
            new DateTime(2020, 04,29 ),
            new DateTime(2020, 04,30 ),
            new DateTime(2020, 5,1 ),
            new DateTime(2020, 5,2 ),
        };

        [Test, Sequential]
        public void AsCurrentWeekTests(
            [Values]DayOfWeek dayOfWeek,
            [ValueSourceAttribute(nameof(CurrentWeekExpectedDates))] DateTime expectedDate)
        {
            var converter = new DayOfWeekToDateTimeConverter(LocalDate);
            var actualDate = converter.Convert(dayOfWeek);
            
            Assert.AreEqual(expectedDate, actualDate);
        }
        
        [Test, Sequential]
        public void AsNextWeekTests(
            [Values]DayOfWeek dayOfWeek,
            [ValueSourceAttribute(nameof(NextWeekExpectedDates))] DateTime expectedDate)
        {
            var converter = new DayOfWeekToDateTimeConverter(LocalDate);
            var actualDate = converter.Convert(dayOfWeek, true);
            
            Assert.AreEqual(expectedDate, actualDate);
        }

        private static ILocalDateService LocalDate => 
            Mock.Of<ILocalDateService>(s => s.LocalDateTime == new DateTime(2020, 04, 26));
    }
}