using System;
using System.Collections.Generic;
using NpuRozklad.Core;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Services.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<DayOfWeek, string> _dayOfWeekToLocalizationShortValue =
            new Dictionary<DayOfWeek, string>
            {
                {DayOfWeek.Monday, "monday-short"},
                {DayOfWeek.Tuesday, "tuesday-short"},
                {DayOfWeek.Wednesday, "wednesday-short"},
                {DayOfWeek.Thursday, "thursday-short"},
                {DayOfWeek.Friday, "friday-short"},
                {DayOfWeek.Saturday, "saturday"},
                {DayOfWeek.Sunday, "sunday"},
            };

        private readonly Dictionary<DayOfWeek, string> _dayOfWeekToLocalizationLongValue =
            new Dictionary<DayOfWeek, string>
            {
                {DayOfWeek.Monday, "monday"},
                {DayOfWeek.Tuesday, "tuesday"},
                {DayOfWeek.Wednesday, "wednesday"},
                {DayOfWeek.Thursday, "thursday"},
                {DayOfWeek.Friday, "friday"},
                {DayOfWeek.Saturday, "saturday"},
                {DayOfWeek.Sunday, "sunday"},
            };

        private readonly Dictionary<string, RozkladLocalization> _localizations;

        public LocalizationService(LocalizationLoader loader)
        {
            _localizations = loader.LoadAll();

            foreach (var rozkladLocalization in _localizations)
            {
                RozkladLocalizationValidator.Validate(rozkladLocalization.Value);
            }
        }

        public LocalizationValue this[string language, string text]
        {
            get
            {
                if (!_localizations.TryGetValue(language, out var rozkladLocalization))
                {
                    rozkladLocalization = _localizations[DefaultLanguage];
                    //log
                }

                return this[rozkladLocalization, text];
            }
        }

        public LocalizationValue this[string language, DayOfWeek dayOfWeek, bool asFullDayName]
        {
            get
            {
                if (!_localizations.TryGetValue(language, out var rozkladLocalization))
                {
                    rozkladLocalization = _localizations[DefaultLanguage];
                    // log? 🐞
                }

                return asFullDayName
                    ? this[rozkladLocalization, _dayOfWeekToLocalizationLongValue[dayOfWeek]]
                    : this[rozkladLocalization, _dayOfWeekToLocalizationShortValue[dayOfWeek]];
            }
        }

        private LocalizationValue this[RozkladLocalization rozkladLocalization, string text]
        {
            get
            {
                if (!rozkladLocalization.Values.TryGetValue(text, out var value))
                {
                    value = _localizations[DefaultLanguage].Values[text];
                    // log
                }

                return new LocalizationValue(value);
            }
        }

        public string DefaultLanguage { get; } = "ua";
    }
}