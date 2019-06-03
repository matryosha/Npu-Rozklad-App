using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NpuTimetableParser;
using RozkladNpuBot.Application.Enums;

namespace RozkladNpuBot.Application.Helpers
{
    public static class CallbackQueryDataConverters
    {
        //TODO return dictionary with values depends on CallbackQueryType
        public static KeyValuePair<CallbackQueryType, List<string>> ConvertDataFromString(string dataString)
        {
            var resultValues = new List<string>();
            var parts = dataString.Split(';');
            if (parts.Length != 1)
            {
                foreach (var part in parts.Skip(1))
                {
                    resultValues.Add(part);
                }
            }
            
            return new 
                KeyValuePair<CallbackQueryType, List<string>>(
                    (CallbackQueryType) int.Parse(parts[0]), resultValues);
        }

        public static string ConvertToDataString(
            CallbackQueryType callbackQueryType,
            params string[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            var result = new StringBuilder();
            result.Append((int)callbackQueryType);
            foreach (var value in values)
            {
                result.Append(';');
                result.Append(value);
            }

            return result.ToString();
        }

        public static string ToDataString(this Group group)
        {
            return $"{group.ShortName};{group.ExternalId};{group.FacultyShortName}";
        }

        public static Group ToGroup(List<string> values)
        {
            return new Group
            {
                ShortName = values[0],
                ExternalId = int.Parse(values[1]),
                FacultyShortName = values[2]
            };
        }

        public static string GetGroupScheduleCallbackData(
            Group group,
            ShowGroupSelectedWeek week,
            int userTelegramId)
        {
            return GetGroupScheduleCallbackData(
                group,
                week,
                DateTime.Today.ToLocal().DayOfWeek,
                userTelegramId);
        }

        //todo better naming?
        public static string GetGroupScheduleCallbackData(
            Group group,
            ShowGroupSelectedWeek week,
            DayOfWeek dayOfWeek,
            int userTelegramId)
        {
            return (int) CallbackQueryType.ShowDetailGroupMenu +
                   $";{group.ToDataString()};{(int)dayOfWeek}" +
                   $";{(int)week};{userTelegramId}";
        }
 

    }


}
