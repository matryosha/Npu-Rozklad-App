using RozkladNpuAspNetCore.Infrastructure.Localization;

namespace RozkladNpuAspNetCore.Interfaces
{
    public interface ILocalizationService
    {
        LocalizationValue this[string language, string text] { get; set; }
    }
}
