using System;

namespace RozkladNpuBot.Application.Helpers
{
    public static class LocalDateTime
    {
        private static readonly TimeZoneInfo LocalTimeZoneInfo;

        static LocalDateTime()
        {
            LocalTimeZoneInfo = 
                TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
        }
        public static DateTime ToLocal(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, LocalTimeZoneInfo);
        }
    }
}
