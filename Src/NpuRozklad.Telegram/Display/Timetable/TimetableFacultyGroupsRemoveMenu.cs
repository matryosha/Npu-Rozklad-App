using System.Collections.Generic;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Display.Common.Controls.FacultyGroupsInlineMenu;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Timetable
{
    public class TimetableFacultyGroupsRemoveMenu
    {
        private readonly IFacultyGroupsInlineMenuCreator _facultyGroupsInlineMenuCreator;
        private readonly BackInlineButtonCreator _backInlineButtonCreator;

        public TimetableFacultyGroupsRemoveMenu(IFacultyGroupsInlineMenuCreator facultyGroupsInlineMenuCreator,
            BackInlineButtonCreator backInlineButtonCreator)
        {
            _facultyGroupsInlineMenuCreator = facultyGroupsInlineMenuCreator;
            _backInlineButtonCreator = backInlineButtonCreator;
        }

        public InlineKeyboardMarkup CreateInlineMenu(ICollection<Group> facultyGroups)
        {
            return _facultyGroupsInlineMenuCreator.CreateMenu(new FacultyGroupsInlineMenuOptions
            {
                FacultyGroups = facultyGroups,
                CallbackActionType = CallbackQueryActionType.RemoveGroup,
                AdditionalButtons = new[]
                    {_backInlineButtonCreator.Create(CallbackQueryActionType.ShowTimetableFacultyGroupsMenu)}
            });
        }
    }
}