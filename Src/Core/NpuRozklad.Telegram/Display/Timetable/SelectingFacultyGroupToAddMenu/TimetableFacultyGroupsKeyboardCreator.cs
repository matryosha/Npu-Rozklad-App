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
            
            var keyboardMarkupMenuOptions = new KeyboardMarkupMenuOptions
            {
                Items = new List<ICollection<string>>(new[] {FacultyGroupsToNameArray(facultyGroups)}),
                AdditionalButtons = new List<KeyboardButton> {_keyboardMarkupMenuCreator.CreateBackButton()}
            };

            return _keyboardMarkupMenuCreator.CreateMenu(keyboardMarkupMenuOptions);
        }

        private static ICollection<string> FacultyGroupsToNameArray(ICollection<Group> facultyGroups)
        {
            return new List<string>(facultyGroups.Select(g => g.Name).ToList());
        }
    }
}