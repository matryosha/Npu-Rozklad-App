using System;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Display.Common.Controls;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Timetable.TimetableFacultyGroupViewMenu
{
    public class TimetableFacultyGroupViewInlineMenuCreator
    {
        private readonly DayOfWeekInlineButtonsCreator _dayOfWeekInlineButtonsCreator;
        private readonly WeekSelectorInlineButtonsCreator _weekSelectorInlineButtonsCreator;
        private readonly BackInlineButtonCreator _backInlineButtonCreator;

        public TimetableFacultyGroupViewInlineMenuCreator(DayOfWeekInlineButtonsCreator dayOfWeekInlineButtonsCreator,
            WeekSelectorInlineButtonsCreator weekSelectorInlineButtonsCreator,
            BackInlineButtonCreator backInlineButtonCreator)
        {
            _dayOfWeekInlineButtonsCreator = dayOfWeekInlineButtonsCreator;
            _weekSelectorInlineButtonsCreator = weekSelectorInlineButtonsCreator;
            _backInlineButtonCreator = backInlineButtonCreator;
        }
        
        public InlineKeyboardMarkup CreateInlineMenu(TimetableFacultyGroupViewInlineMenuCreatorOptions options)
        {
            var facultyGroup = options.FacultyGroup;
            var activeDayOfWeek = options.ActiveDayOfWeek;
            var isNextWeekActive = options.IsNextWeekActive;

            var daysOfWeekButtons = _dayOfWeekInlineButtonsCreator.Create(new DayOfWeekInlineButtonsCreatorOptions
            {
                IsNextWeekSelected = isNextWeekActive,
                DayOfWeekToMark = activeDayOfWeek,
                FacultyGroup = facultyGroup
            });

            var weekButtons = _weekSelectorInlineButtonsCreator.Create(new WeekSelectorInlineButtonsCreatorOptions
            {
                ActiveDayOfWeek = activeDayOfWeek,
                FacultyGroup = facultyGroup,
                IsNextWeekActive = isNextWeekActive
            });

            var backButton = _backInlineButtonCreator.Create(CallbackQueryActionType.ShowTimetableFacultyGroupsMenu);
            
            return new[]
            {
                daysOfWeekButtons,
                weekButtons,
                new[] {backButton}
            };

        }
    }

    public class TimetableFacultyGroupViewInlineMenuCreatorOptions
    {
        public Group FacultyGroup { get; set; }
        public DayOfWeek ActiveDayOfWeek { get; set; }
        public bool IsNextWeekActive { get; set; }
    }
}