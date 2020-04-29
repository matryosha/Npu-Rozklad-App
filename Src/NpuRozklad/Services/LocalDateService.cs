using System;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Services
{
    public class LocalDateService : ILocalDateService
    {
        private static readonly TimeZoneInfo LocalTimeZoneInfo;

        static LocalDateService()
        {
            try
            {
                LocalTimeZoneInfo =
                    TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            }
            catch (TimeZoneNotFoundException e)
            {
                LocalTimeZoneInfo = 
                    TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev");
            }
        }
        public DateTime LocalDateTime => TimeZoneInfo.ConvertTime(DateTime.Now, LocalTimeZoneInfo);
    }
}