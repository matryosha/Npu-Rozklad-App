using System;
using NpuRozklad.Core;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Interfaces;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Services
{
    public class CurrentUserLocalizationService : ICurrentUserLocalizationService
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentScopeServiceProvider _currentScopeServiceProvider;

        private Func<string> _getUserLang;
        private string CurrentUserLanguage => _getUserLang();

        public CurrentUserLocalizationService(ILocalizationService localizationService,
            ICurrentScopeServiceProvider currentScopeServiceProvider)
        {
            _localizationService = localizationService;
            _currentScopeServiceProvider = currentScopeServiceProvider;

            _getUserLang = () => _currentScopeServiceProvider.GetService<ICurrentTelegramUserContext>().Language;
        }

        public LocalizationValue this[string language, string text] => _localizationService[language, text];

        public LocalizationValue this[string language, DayOfWeek dayOfWeek, bool asFullDayName] =>
            _localizationService[language, dayOfWeek, asFullDayName];

        public string DefaultLanguage => _localizationService.DefaultLanguage;

        public LocalizationValue this[string text] => _localizationService[CurrentUserLanguage, text];

        public LocalizationValue this[DayOfWeek dayOfWeek, bool asFullDayName] => 
            _localizationService[CurrentUserLanguage, dayOfWeek, asFullDayName];

        public ICurrentUserLocalizationService GetWithBoundUserLanguage()
        {
            var localizationService =
                new CurrentUserLocalizationService(_localizationService, _currentScopeServiceProvider);
            var userLanguage = CurrentUserLanguage;
            localizationService._getUserLang = () => userLanguage;
            return localizationService;
        }
    }
}