using System.Collections.Generic;
using System.Linq;
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
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public MainMenuCreator(KeyboardMarkupMenuCreator keyboardMarkupMenuCreator,
        ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _keyboardMarkupMenuCreator = keyboardMarkupMenuCreator;
            _currentUserLocalizationService = currentUserLocalizationService;
        }

        public ReplyKeyboardMarkup CreateMenu(MainMenuOptions options = null)
        {
            var menuItemsText = new List<string>(MainMenuItems.Count);
            menuItemsText.AddRange(MainMenuItems
                .Select(localizationValueName =>
                    (string) _currentUserLocalizationService[localizationValueName]));
            
            var keyboardMarkupMenuOptions = new KeyboardMarkupMenuOptions
            {
                Items = new List<ICollection<string>>{menuItemsText},
                OneTimeKeyboard = true,
                ResizeKeyboard = true
            };
            
            return _keyboardMarkupMenuCreator.CreateMenu(keyboardMarkupMenuOptions);
        }
    }

    public class MainMenuOptions
    {
    }
}