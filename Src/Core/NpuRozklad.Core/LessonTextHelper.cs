namespace NpuRozklad.Core
{
    public static class LessonTextHelper
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
                    return "17:00 - 18:20";
                default: return number.ToString();
            }
        }
    }
}