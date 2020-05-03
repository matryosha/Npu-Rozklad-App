using System;
using System.Text;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Telegram.Helpers
{
    internal static class TimetableFacultyGroupViewMenuCallbackDataSerializer
    {
        internal static string ToCallbackData(bool isNextWeek, DayOfWeek dayOfWeek, Group facultyGroup)
        {
            var builder = new StringBuilder();

            builder.AppendWithSeparator(
                CallbackDataFormatter.ToCallBackData(CallbackQueryActionType.ShowTimetableFacultyGroupViewMenu));
            builder.AppendWithSeparator(isNextWeek ? "1" : "0");
            builder.AppendWithSeparator(((int) dayOfWeek).ToString());
            builder.AppendWithSeparator(
                CallbackDataFormatter.ToCallBackData(facultyGroup));

            return builder.ToString();
        }
    }
}