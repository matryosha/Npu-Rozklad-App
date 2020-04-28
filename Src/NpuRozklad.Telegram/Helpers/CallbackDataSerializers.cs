using System.Text;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Telegram.Helpers
{
    internal static class TimetableFacultyGroupViewMenuCallbackDataSerializer
    {
        internal static string ToCallbackData(bool isNextWeek, int dayNumber, Group facultyGroup)
        {
            var builder = new StringBuilder();

            builder.AppendWithSeparator(
                CallbackDataFormatter.ToCallBackData(CallbackQueryActionType.ShowTimetableFacultyGroupViewMenu));
            builder.AppendWithSeparator((isNextWeek ? 0 : 1).ToString());
            builder.AppendWithSeparator(dayNumber.ToString());
            builder.AppendWithSeparator(
                CallbackDataFormatter.ToCallBackData(facultyGroup));

            return builder.ToString();
        }
    }
}