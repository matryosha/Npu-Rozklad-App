using System.Collections.Generic;
using System.Linq;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Display.Common.Controls.KeyboardMarkupMenu;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Timetable.SelectingFacultyMenu
{
    public class TimetableFacultyListKeyboardCreator
    {
        private readonly KeyboardMarkupMenuCreator _keyboardMarkupMenuCreator;

        public TimetableFacultyListKeyboardCreator(
            KeyboardMarkupMenuCreator keyboardMarkupMenuCreator)
        {
            _keyboardMarkupMenuCreator = keyboardMarkupMenuCreator;
        }

        public ReplyKeyboardMarkup CreateMarkup(TimetableFacultyListKeyboardOptions options)
        {
            var faculties = options.Faculties;
            var facultiesNames = FacultiesToNameArray(faculties);

            var keyboardMarkupMenuOptions = new KeyboardMarkupMenuOptions
            {
                Items = new List<ICollection<string>>(facultiesNames.Count),
                AdditionalButtons = new List<KeyboardButton> {_keyboardMarkupMenuCreator.CreateBackButton()},
                OneTimeKeyboard = true
            };

            foreach (var name in facultiesNames)
            {
                keyboardMarkupMenuOptions.Items.Add(new[] {name});
            }
            
            return _keyboardMarkupMenuCreator.CreateMenu(keyboardMarkupMenuOptions);
        }

        private static ICollection<string> FacultiesToNameArray(ICollection<Faculty> faculties)
        {
            return new List<string>(faculties.Select(f => f.FullName).ToArray());
        }
    }
}