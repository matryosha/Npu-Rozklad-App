using System;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Common.Text;
using NpuRozklad.Telegram.Display.Timetable.TimetableFacultyGroupViewMenu;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.Enums;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowTimetableFacultyGroupViewMenuAction
    {
        private readonly TimetableFacultyGroupViewInlineMenuCreator _menuCreator;
        private readonly IDayOfWeekToDateTimeConverter _dayOfWeekToDateTimeConverter;
        private readonly ILessonsProvider _lessonsProvider;
        private readonly OneDayLessonsToTelegramMessageText _lessonsToTelegramMessageText;
        private readonly ITelegramBotService _telegramBotService;

        public ShowTimetableFacultyGroupViewMenuAction(
            TimetableFacultyGroupViewInlineMenuCreator menuCreator,
            IDayOfWeekToDateTimeConverter dayOfWeekToDateTimeConverter,
            ILessonsProvider lessonsProvider,
            OneDayLessonsToTelegramMessageText lessonsToTelegramMessageText,
            ITelegramBotService telegramBotService)
        {
            _menuCreator = menuCreator;
            _dayOfWeekToDateTimeConverter = dayOfWeekToDateTimeConverter;
            _lessonsProvider = lessonsProvider;
            _lessonsToTelegramMessageText = lessonsToTelegramMessageText;
            _telegramBotService = telegramBotService;
        }

        public async Task Execute(ShowTimetableFacultyGroupViewMenuOptions options)
        {
            var facultyGroup = options.FacultyGroup;
            var dayOfWeek = options.DayOfWeek;
            var isNextWeekSelected = options.IsNextWeekSelected;

            var inlineKeyboard = _menuCreator.CreateInlineMenu(
                new TimetableFacultyGroupViewInlineMenuCreatorOptions
                {
                    FacultyGroup = facultyGroup,
                    ActiveDayOfWeek = dayOfWeek,
                    IsNextWeekActive = isNextWeekSelected
                });

            var lessonsDateTime = _dayOfWeekToDateTimeConverter.Convert(dayOfWeek, isNextWeekSelected);

            var lessons = await _lessonsProvider.GetLessonsOnDate(facultyGroup, lessonsDateTime);

            var messageText = _lessonsToTelegramMessageText.CreateMessage(
                new OneDayLessonsToTelegramMessageTextOptions
                {
                    FacultyGroup = facultyGroup,
                    LessonsDate = lessonsDateTime,
                    OneDayLessons = lessons
                });

            await _telegramBotService.SendOrEditMessageAsync(
                messageText,
                ParseMode.Html,
                replyMarkup: inlineKeyboard);
        }
    }

    public class ShowTimetableFacultyGroupViewMenuOptions
    {
        public Group FacultyGroup { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public bool IsNextWeekSelected { get; set; }
    }
}