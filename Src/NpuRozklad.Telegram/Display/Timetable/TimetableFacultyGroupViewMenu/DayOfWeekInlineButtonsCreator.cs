using System;
using NpuRozklad.Core.Entities;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using static NpuRozklad.Core.Infrastructure.NpuDateTimeHelper;
using static NpuRozklad.Telegram.Display.Common.Text.TextDecoration;
using static NpuRozklad.Telegram.Helpers.TimetableFacultyGroupViewMenuCallbackDataSerializer;

namespace NpuRozklad.Telegram.Display.Timetable.TimetableFacultyGroupViewMenu
{
    public class DayOfWeekInlineButtonsCreator
    {
        private readonly InlineKeyboardButtonsCreator _inlineKeyboardButtonsCreator;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public DayOfWeekInlineButtonsCreator(InlineKeyboardButtonsCreator inlineKeyboardButtonsCreator,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _inlineKeyboardButtonsCreator = inlineKeyboardButtonsCreator;
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        
        public InlineKeyboardButton[] Create(DayOfWeekInlineButtonsCreatorOptions options)
        {
            var isNextWeek = options.IsNextWeekSelected;
            var shouldMarkDayOfWeek = options.ShouldMarkDayOfWeek;
            var dayOfWeekToMark = options.DayOfWeekToMark;
            var facultyGroup = options.FacultyGroup;

            var result = _inlineKeyboardButtonsCreator.Create(o =>
            {
                o.ItemsNumber = 5;
                o.ButtonTextFunc = i => _currentUserLocalizationService[DayOfWeekNumberToLocalDayOfWeek(i)];
                o.CallbackDataFunc = i =>
                    ToCallbackData(isNextWeek, DayOfWeekNumberToLocalDayOfWeek(i), facultyGroup);
            })[0];

            if (shouldMarkDayOfWeek)
            {
                if (dayOfWeekToMark != DayOfWeek.Saturday && dayOfWeekToMark != DayOfWeek.Sunday)
                {
                    var button = result[DayOfWeekToLocal(dayOfWeekToMark)];
                    button.Text = $"{ActiveMark} {button.Text}";
                }
            }

            return result;
        }
    }

    public class DayOfWeekInlineButtonsCreatorOptions
    {
        public bool ShouldMarkDayOfWeek { get; set; } = true;
        public DayOfWeek DayOfWeekToMark { get; set; }
        public bool IsNextWeekSelected { get; set; }
        public Group FacultyGroup { get; set; }
    }
}