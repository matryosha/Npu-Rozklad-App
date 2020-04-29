using System;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Display.Common.Text;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using static NpuRozklad.Telegram.Helpers.TimetableFacultyGroupViewMenuCallbackDataSerializer;

namespace NpuRozklad.Telegram.Display.Timetable.TimetableFacultyGroupViewMenu
{
    public class WeekSelectorInlineButtonsCreator
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentUserService;
        private readonly InlineKeyboardButtonsCreator _inlineKeyboardButtonsCreator;

        public WeekSelectorInlineButtonsCreator(
            ILocalizationService localizationService,
            ICurrentTelegramUserService currentUserService,
            InlineKeyboardButtonsCreator inlineKeyboardButtonsCreator)
        {
            _localizationService = localizationService;
            _currentUserService = currentUserService;
            _inlineKeyboardButtonsCreator = inlineKeyboardButtonsCreator;
        }

        public InlineKeyboardButton[] Create(WeekSelectorInlineButtonsCreatorOptions options)
        {
            var isNextWeekActive = options.IsNextWeekActive;
            var activeDayOfWeek = (int) options.ActiveDayOfWeek;
            var facultyGroup = options.FacultyGroup;
            var buttonsText = GetButtonsText();

            var buttons = _inlineKeyboardButtonsCreator.Create(new InlineKeyboardButtonsCreatorOptions
            {
                NumberOfButtons = 2,
                ButtonTextFunc = i => buttonsText[i],
                CallbackDataFunc = i => ToCallbackData(isNextWeekActive, (DayOfWeek) activeDayOfWeek, facultyGroup)
            });

            var buttonIndexToChange = isNextWeekActive ? 1 : 0;
            var button = buttons[buttonIndexToChange];
            button.Text = $"{TextDecoration.ActiveMark} {button.Text}";

            return buttons;
        }

        private string[] GetButtonsText()
        {
            var result = new string[2];

            result[0] = _localizationService[_currentUserService.Language, "this-week"];
            result[1] = _localizationService[_currentUserService.Language, "next-week"];

            return result;
        }
    }

    public class WeekSelectorInlineButtonsCreatorOptions
    {
        public bool IsNextWeekActive { get; set; }
        public DayOfWeek ActiveDayOfWeek { get; set; }
        public Group FacultyGroup { get; set; }
    }
}