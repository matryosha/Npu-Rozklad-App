using System;

namespace RozkladNpuBot.Common
{
    public static class LocalDateTime
    {
        private static readonly TimeZoneInfo LocalTimeZoneInfo;

        static LocalDateTime()
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
        public static DateTime ToLocal(this DateTime date)
        {
            return TimeZoneInfo.ConvertTime(date, LocalTimeZoneInfo);
        }
    }
}
