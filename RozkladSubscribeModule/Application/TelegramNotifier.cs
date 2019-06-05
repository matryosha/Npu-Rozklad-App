using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RozkladNpuBot.Infrastructure;
using RozkladNpuBot.Infrastructure.Interfaces;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;
using Telegram.Bot.Types;

namespace RozkladSubscribeModule.Application
{
    internal class TelegramNotifier :
        IUserNotifyService<DefaultNotifyPayload>
    {
        private readonly ILogger<TelegramNotifier> _logger;
        private readonly IBotService _botService;
        private readonly ILocalizationService _localizationService;
        private readonly MessageBuilderService _messageBuilderService;

        public TelegramNotifier(
            ILogger<TelegramNotifier> logger,
            IBotService botService,
            ILocalizationService localizationService,
            MessageBuilderService messageBuilderService)
        {
            _logger = logger;
            _botService = botService;
            _localizationService = localizationService;
            _messageBuilderService = messageBuilderService;
        }

        public async Task Notify(SubscribedUser subscribedUser, DefaultNotifyPayload payload)
        {
            var datesWithChangedSchedule = new HashSet<DateTime>();
            foreach (var lesson in payload.UpdatedLessons) {
                datesWithChangedSchedule.Add(lesson.LessonDate);
            }

            var messageHeader =
                $"{_localizationService["ua", "schedule-was-updated-for"]}" +
                $" {subscribedUser.GroupShortName}{Environment.NewLine}";

            var datesString = new StringBuilder();
            datesString.Append(_localizationService["ua", "dates-with-updated-schedule"] + ':');
            datesString.Append(Environment.NewLine);
            foreach (var date in datesWithChangedSchedule)
            {
                datesString.Append(_messageBuilderService.ConvertDayOfWeekToText(date.DayOfWeek));
                datesString.Append(' ');
                datesString.Append(date.ToString("dd/MM"));
                datesString.Append(Environment.NewLine);
            }

            var messageText = messageHeader + datesString;
            
            await _botService.Client.SendTextMessageAsync(
                new ChatId(subscribedUser.ChatId), messageText);
        }
    }
}