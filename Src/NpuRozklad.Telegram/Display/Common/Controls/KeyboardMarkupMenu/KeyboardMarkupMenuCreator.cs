using System.Collections.Generic;
using System.Linq;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls.KeyboardMarkupMenu
{
    public class KeyboardMarkupMenuCreator
    {
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public KeyboardMarkupMenuCreator(ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        public ReplyKeyboardMarkup CreateMenu(KeyboardMarkupMenuOptions options)
        {
            var items = options.Items;
            var additionalButtons = options.AdditionalButtons;
            var oneTimeKeyboard = options.OneTimeKeyboard;
            var resizeKeyboard = options.ResizeKeyboard;
            
            var rows = new List<ICollection<KeyboardButton>>(items.Count);
            var currentRowButtons = new List<KeyboardButton>();

            foreach (var rowItems in items)
            {
                currentRowButtons.AddRange(rowItems.Select(CreateButton));

                rows.Add(currentRowButtons);
                currentRowButtons = new List<KeyboardButton>();
            }
            
            rows.Add(additionalButtons);

            return new ReplyKeyboardMarkup
            {
                Keyboard = rows,
                OneTimeKeyboard = oneTimeKeyboard,
                ResizeKeyboard = resizeKeyboard
            };
        }
        
        public KeyboardButton CreateBackButton() => new KeyboardButton
        {
            Text = _currentUserLocalizationService["back"]
        };
        private static KeyboardButton CreateButton(string text) => new KeyboardButton {Text = text};
    }
}