using System;
using NpuRozklad.Core;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Telegram.Services.Interfaces
{
    public interface ICurrentUserLocalizationService : ILocalizationService
    {
        LocalizationValue this[string text] { get; }
        LocalizationValue this[DayOfWeek dayOfWeek, bool asFullDayName = false] { get; }
    }
}