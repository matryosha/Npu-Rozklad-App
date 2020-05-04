using System.Collections.Generic;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Display.Common.Controls.FacultyGroupsInlineMenu;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

using static NpuRozklad.Telegram.CallbackQueryActionType;
using static NpuRozklad.Telegram.Helpers.CallbackDataFormatter;

namespace NpuRozklad.Telegram.Display.Timetable
{
    public class TimetableFacultyGroupsMenu
    {
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;
        private readonly IFacultyGroupsInlineMenuCreator _facultyGroupsInlineMenuCreator;

        public TimetableFacultyGroupsMenu(
            ICurrentUserLocalizationService currentUserLocalizationService,
            IFacultyGroupsInlineMenuCreator facultyGroupsInlineMenuCreator)
        {
            _currentUserLocalizationService = currentUserLocalizationService;
            _facultyGroupsInlineMenuCreator = facultyGroupsInlineMenuCreator;
        }
        
        public InlineKeyboardMarkup CreateInlineMenu(ICollection<Group> facultyGroups)
        {
            var facultyGroupsInlineMenuOptions = new FacultyGroupsInlineMenuOptions
            {
                FacultyGroups = facultyGroups,
                CallbackActionType = TimetableFacultyGroupsMenuGroupSelected,
                AdditionalButtons = new []{CreateAddGroupButton(), CreateRemoveGroupButton()}
            };

            return _facultyGroupsInlineMenuCreator.CreateMenu(facultyGroupsInlineMenuOptions);
        }

        private InlineKeyboardButton CreateAddGroupButton()
        {
            return new InlineKeyboardButton
            {
                Text = _currentUserLocalizationService["add-group"],
                CallbackData = ToCallBackData(AddGroup)
            };
        }

        private InlineKeyboardButton CreateRemoveGroupButton()
        {
            return new InlineKeyboardButton()
            {
                Text = _currentUserLocalizationService["remove-group"],
                CallbackData = ToCallBackData(OpenRemoveGroupsMenu)
            };
        }
    }
}