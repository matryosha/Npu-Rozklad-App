using RozkladNpuBot.Application.Localization;

namespace RozkladNpuBot.Application.Interfaces
{
    public interface ILocalizationService
    {
        LocalizationValue this[string language, string text] { get; }
    }
}
