using RozkladNpuAspNetCore.Infrastructure.Localization;

namespace RozkladNpuAspNetCore.Extensions
{
    public static class LocalizationValueExtensions
    {
        public static string AsActive(this LocalizationValue value)
        {
            return $"▶ {value}";
        }
    }
}
