using System;

namespace NpuRozklad.Core.Interfaces
{
    public interface ILocalizationService
    {
        LocalizationValue this[string language, string text] { get; }
        LocalizationValue this[string language, DayOfWeek dayOfWeek, bool asFullDayName = false] { get; }
        string DefaultLanguage { get; }
    }
}
