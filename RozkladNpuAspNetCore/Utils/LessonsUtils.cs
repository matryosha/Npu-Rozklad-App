using System;
using System.Collections.Generic;
using System.Text;

namespace RozkladNpuBot.Utils
{
    class LessonsUtils
    {
        public static string ConvertLessonNumberToText(int number)
        {
            switch (number)
            {
                case 1:
                    return "8:00 - 9:20";
                case 2:
                    return "9:30 - 10:50";
                case 3:
                    return "11:00 - 12:20";
                case 4:
                    return "12:30 - 13:50";
                case 5:
                    return "14:00 - 15:20";
                case 6:
                    return "15:30 - 16:50";
                case 7:
                    return "воу 7 пара, эх **nema**";
                default: return number.ToString();
            }
        }

        public static string ConvertDayOfWeekToText(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Понедельник";
                case DayOfWeek.Tuesday:
                    return "Вторник";
                case DayOfWeek.Wednesday:
                    return "Среду";
                case DayOfWeek.Thursday:
                    return "Четверг";
                case DayOfWeek.Friday:
                    return "Пятницу";
                case DayOfWeek.Saturday:
                    return "Субботу";
                case DayOfWeek.Sunday:
                    return "Воскресение";
            }

            return "??";
        }
    }
}
