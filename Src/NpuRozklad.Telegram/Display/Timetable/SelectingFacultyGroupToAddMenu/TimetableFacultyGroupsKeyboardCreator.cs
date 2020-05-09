using System.Collections.Generic;
using System.Linq;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Display.Common.Controls.KeyboardMarkupMenu;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Timetable.SelectingFacultyGroupToAddMenu
{
    public class TimetableFacultyGroupsKeyboardCreator
    {
        private readonly KeyboardMarkupMenuCreator _keyboardMarkupMenuCreator;

        public TimetableFacultyGroupsKeyboardCreator(KeyboardMarkupMenuCreator keyboardMarkupMenuCreator)
        {
            _keyboardMarkupMenuCreator = keyboardMarkupMenuCreator;
        }
        
        public ReplyKeyboardMarkup CreateMarkup(TimetableFacultyGroupsKeyboardOptions options)
        {
            var facultyGroups = options.FacultyGroups;
            var facultyGroupsNames = FacultyGroupsToNameArray(facultyGroups);
            
            var keyboardMarkupMenuOptions = new KeyboardMarkupMenuOptions
            {
                Items = new List<ICollection<string>>(facultyGroupsNames.Count),
                AdditionalButtons = new List<KeyboardButton> {_keyboardMarkupMenuCreator.CreateBackButton()},
                OneTimeKeyboard = true
            };
            
            foreach (var name in facultyGroupsNames)
            {
                keyboardMarkupMenuOptions.Items.Add(new[] {name});
            }
            
            return _keyboardMarkupMenuCreator.CreateMenu(keyboardMarkupMenuOptions);
        }

        private static ICollection<string> FacultyGroupsToNameArray(ICollection<Group> facultyGroups)
        {
            return new List<string>(facultyGroups.Select(g => g.Name).ToArray());
        }
    }
}