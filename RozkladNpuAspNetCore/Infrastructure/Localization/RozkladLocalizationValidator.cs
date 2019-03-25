using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RozkladNpuAspNetCore.Exceptions;

namespace RozkladNpuAspNetCore.Infrastructure.Localization
{
    public static class RozkladLocalizationValidator
    {
        private static readonly string[] _localizationValuesKeys = 
        {
            "choose-group-message",
            "choose-action-message",
            "schedule-reply-keyboard",
            "menu-reply-keyboard",
            "choose-faculty-message",
            "no-groups-for-faculty-message",
            "add-group",
            "monday",
            "tuesday",
            "wednesday",
            "thursday",
            "friday",
            "saturday",
            "sunday",
            "monday-short",
            "tuesday-short",
            "wednesday-short",
            "thursday-short",
            "friday-short",
            "this-week",
            "next-week",
            "back",
            "delete",
            "classes-on",
            "updated-on",
            "group"
        };
        public static void Validate(
            RozkladLocalization rozkladLocalization)
        {
            if (string.IsNullOrEmpty(rozkladLocalization.ShortName))
            {
                throw new Exception(
                    "Short name is not specified in localization file");
            }
            if (string.IsNullOrEmpty(rozkladLocalization.FullName))
            {
                throw new RozkladLocalizationValidationException(
                    $"Full name is not specified in {rozkladLocalization.ShortName} localization");
            }

            var missingKeys = new List<string>();
            var emptyKeys = new List<string>();
            foreach (var checkKey in _localizationValuesKeys)
            {
                if (rozkladLocalization.Values.TryGetValue(checkKey, out string value))
                {
                    if(string.IsNullOrEmpty(value))
                        emptyKeys.Add(checkKey);
                }
                else
                {
                    missingKeys.Add(checkKey);
                }
            }
            if(!missingKeys.Any() && !emptyKeys.Any())
                return;

            var exceptionMessage = CreateExceptionMessage(
                missingKeys, emptyKeys, rozkladLocalization.ShortName);

            throw new RozkladLocalizationValidationException(exceptionMessage);
        }

        private static string CreateExceptionMessage(
            List<string> missingKeys, 
            List<string> emptyKeys,
            string localizationName)
        {
            var resultMessage = new StringBuilder();
            foreach (var missingKey in missingKeys)
            {
                resultMessage.AppendLine($"Key {missingKey} is missing in {localizationName}");

            }
            foreach (var emptyKey in emptyKeys)
            {
                resultMessage.AppendLine($"Key {emptyKey} is defined but empty in {localizationName}");
            }

            return resultMessage.ToString();
        }
    }
}
