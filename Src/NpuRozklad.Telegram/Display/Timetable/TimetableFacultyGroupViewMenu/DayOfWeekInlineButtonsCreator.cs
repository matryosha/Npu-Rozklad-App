using System;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
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
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentUserService;
        private readonly InlineKeyboardButtonsCreator _inlineKeyboardButtonsCreator;
        private string Lang => _currentUserService.Language;

        public DayOfWeekInlineButtonsCreator(
            ILocalizationService localizationService,
            ICurrentTelegramUserService currentUserService,
            InlineKeyboardButtonsCreator inlineKeyboardButtonsCreator)
        {
            _localizationService = localizationService;
            _currentUserService = currentUserService;
            _inlineKeyboardButtonsCreator = inlineKeyboardButtonsCreator;
        }
        
        public InlineKeyboardButton[] Create(DayOfWeekInlineButtonsCreatorOptions options)
        {
            var isNextWeek = options.IsNextWeekSelected;
            var shouldMarkDayOfWeek = options.ShouldMarkDayOfWeek;
            var dayOfWeekToMark = options.DayOfWeekToMark;
            var facultyGroup = options.FacultyGroup;

            var result = _inlineKeyboardButtonsCreator.Create(new InlineKeyboardButtonsCreatorOptions
            {
                NumberOfButtons = 5,
                ButtonTextFunc = i => _localizationService[Lang, DayOfWeekNumberToLocalDayOfWeek(i)],
                CallbackDataFunc = i =>
                    ToCallbackData(isNextWeek, DayOfWeekNumberToLocalDayOfWeek(i), facultyGroup)
            });

            if (shouldMarkDayOfWeek)
            {
                var button = result[(int) dayOfWeekToMark];
                button.Text = $"{ActiveMark} {button.Text}";
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