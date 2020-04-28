using System.Collections.Generic;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Common.Controls.FacultyGroupsInlineMenu;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

using static NpuRozklad.Telegram.CallbackQueryActionType;
using static NpuRozklad.Telegram.Helpers.CallbackDataFormatter;

namespace NpuRozklad.Telegram.Display.Timetable
{
    public class TimetableFacultyGroupsMenu
    {
        private readonly ICurrentTelegramUserService _currentTelegramUser;
        private readonly ILocalizationService _localizationService;
        private readonly IFacultyGroupsInlineMenuCreator _facultyGroupsInlineMenuCreator;

        public TimetableFacultyGroupsMenu(ICurrentTelegramUserService currentTelegramUser,
            ILocalizationService localizationService,
            IFacultyGroupsInlineMenuCreator facultyGroupsInlineMenuCreator)
        {
            this._currentTelegramUser = currentTelegramUser;
            _localizationService = localizationService;
            _facultyGroupsInlineMenuCreator = facultyGroupsInlineMenuCreator;
        }
        
        public InlineKeyboardMarkup CreateInlineMenu(ICollection<Group> facultyGroups)
        {
            var facultyGroupsInlineMenuOptions = new FacultyGroupsInlineMenuOptions
            {
                FacultyGroups = facultyGroups,
                CallbackActionType = ShowTimetableFacultyGroupsMenu,
                AdditionalButtons = new []{CreateAddGroupButton()}
            };

            return _facultyGroupsInlineMenuCreator.CreateMenu(facultyGroupsInlineMenuOptions);
        }

        private InlineKeyboardButton CreateAddGroupButton()
        {
            return new InlineKeyboardButton
            {
                Text = _localizationService[_currentTelegramUser.Language, "add-group"],
                CallbackData = ToCallBackData(AddGroup)
            };
        }
    }
}