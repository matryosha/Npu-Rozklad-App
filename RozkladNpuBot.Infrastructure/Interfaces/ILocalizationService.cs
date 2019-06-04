using RozkladNpuBot.Infrastructure.Localization;

namespace RozkladNpuBot.Infrastructure.Interfaces
{
    public interface ILocalizationService
    {
        LocalizationValue this[string language, string text] { get; }
    }
}
