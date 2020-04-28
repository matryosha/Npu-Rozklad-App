using System.Collections.Generic;
using System.Linq;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Common.Controls.KeyboardMarkupMenu;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls
{
    public class MainMenuCreator
    {
        private static readonly List<string> MainMenuItems = new List<string>
        {
            "schedule-reply-keyboard"
        };

        private readonly KeyboardMarkupMenuCreator _keyboardMarkupMenuCreator;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentTelegramUserService;

        public MainMenuCreator(KeyboardMarkupMenuCreator keyboardMarkupMenuCreator,
            ILocalizationService localizationService,
            ICurrentTelegramUserService currentTelegramUserService)
        {
            _keyboardMarkupMenuCreator = keyboardMarkupMenuCreator;
            _localizationService = localizationService;
            _currentTelegramUserService = currentTelegramUserService;
        }

        public ReplyKeyboardMarkup CreateMenu(MainMenuOptions options = null)
        {
            var menuItemsText = new List<string>(MainMenuItems.Count);
            menuItemsText.AddRange(MainMenuItems
                .Select(localizationValueName =>
                    (string) _localizationService[_currentTelegramUserService.Language, localizationValueName]));
            
            var keyboardMarkupMenuOptions = new KeyboardMarkupMenuOptions
            {
                Items = new List<ICollection<string>>{menuItemsText}
            };
            
            return _keyboardMarkupMenuCreator.CreateMenu(keyboardMarkupMenuOptions);
        }
    }

    public class MainMenuOptions
    {
    }
}