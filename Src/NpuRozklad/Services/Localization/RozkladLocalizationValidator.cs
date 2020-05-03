using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NpuRozklad.Exceptions;

namespace NpuRozklad.Services.Localization
{
    public static class RozkladLocalizationValidator
    {
        private static readonly string[] LocalizationValuesKeys = 
        {
            "choose-group-message",
            "choose-action-message",
            "schedule-reply-keyboard",
            "menu-reply-keyboard",
            "choose-faculty-message",
            "no-groups-for-faculty-message",
            "add-group",
            "remove-group",
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
            "group",
            "menu-notification",
            "notification-for",
            "turn-on",
            "turn-off",
            "notifications-enabled-message",
            "notifications-disabled-message",
            "schedule-was-updated-for",
            "dates-with-updated-schedule",
            "incorrect-input",
            "faculty-group-has-been-added"
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
                throw new Exception(
                    $"Full name is not specified in {rozkladLocalization.ShortName} localization");
            }

            var missingKeys = new List<string>();
            var emptyKeys = new List<string>();
            foreach (var checkKey in LocalizationValuesKeys)
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
            
            throw RozkladLocalizationValidationException.Create(missingKeys, emptyKeys, rozkladLocalization.ShortName);
        }
    }
}
