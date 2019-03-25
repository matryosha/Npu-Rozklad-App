using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RozkladNpuAspNetCore.Exceptions;
using RozkladNpuAspNetCore.Infrastructure.Localization;
using RozkladNpuAspNetCore.Interfaces;

namespace RozkladNpuAspNetCore.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly List<RozkladLocalization> _localizations;
        public LocalizationService(
            ILogger<LocalizationService> logger)
        {
            _localizations = LocalizationLoader.LoadAll();

            foreach (var rozkladLocalization in _localizations)
            {
                try
                {
                    RozkladLocalizationValidator.Validate(rozkladLocalization);
                }
                catch (RozkladLocalizationValidationException e)
                {
                    logger.LogWarning("Localization validation exception", e);
                }
            }
            
        }

        public LocalizationValue this[string language, string text]
        {
            get
            {
                var localization = _localizations.FirstOrDefault(l => l.ShortName == language);
                if (localization == null)
                    throw new Exception($"Localization for {language} is not defined");
                if (!localization.Values.TryGetValue(text, out string value))
                    return new LocalizationValue(string.Empty);

                return new LocalizationValue(value);
            }
        }
    }
}
