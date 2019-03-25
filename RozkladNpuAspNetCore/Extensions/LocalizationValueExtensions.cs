using RozkladNpuAspNetCore.Infrastructure.Localization;

namespace RozkladNpuAspNetCore.Extensions
{
    public static class LocalizationValueExtensions
    {
        public static string Active(this LocalizationValue value)
        {
            return $"▶ {value}";
        }
    }
}
