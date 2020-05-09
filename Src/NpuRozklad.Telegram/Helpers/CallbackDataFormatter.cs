using System;
using System.Text;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Handlers.CallbackQueryHandlers;

namespace NpuRozklad.Telegram.Helpers
{
    internal static class CallbackDataFormatter
    {
        internal static char S => c_callbackDataValueSeparator;
        internal const char c_callbackDataValueSeparator = ';';

        internal static string ToCallBackData(CallbackQueryActionType actionType)
        {
            return $"{(int)actionType}";
        }

        internal static string ToCallBackData(CallbackQueryActionType actionType, Group facultyGroup)
        {
            return $"{(int)actionType}{S}{ToCallBackData(facultyGroup)}";
        }

        internal static string ToCallBackData(Group facultyGroup) =>
            $"{facultyGroup.TypeId}{S}{facultyGroup.Faculty.TypeId}";

        internal static CallbackQueryData DeserializeCallbackQueryData(string callbackQueryDataString)
        {
            var values = callbackQueryDataString.Split(new[] {S}, StringSplitOptions.RemoveEmptyEntries);
            var callbackQueryData = new CallbackQueryData
            {
                CallbackQueryActionType = (CallbackQueryActionType) int.Parse(values[0]),
                Values = new string[values.Length - 1]
            };
            
            for (var i = 1; i < values.Length; i++)
            {
                callbackQueryData.Values[i-1] = values[i];
            }

            return callbackQueryData;
        }

        internal static StringBuilder AppendWithSeparator(this StringBuilder builder, string value)
        {
            builder.Append(value);
            builder.Append(c_callbackDataValueSeparator);
            return builder;
        }
    }
}