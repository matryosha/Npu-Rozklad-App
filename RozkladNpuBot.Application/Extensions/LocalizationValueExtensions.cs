using RozkladNpuBot.Infrastructure.Localization;

namespace RozkladNpuBot.Application.Extensions
{
    public static class LocalizationValueExtensions
    {
        public static string AsActive(this LocalizationValue value)
        {
            return $"▶ {value}";
        }
    }
}
