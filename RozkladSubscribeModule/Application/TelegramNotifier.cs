using System;
using System.Collections.Generic;
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

        public TelegramNotifier(
            ILogger<TelegramNotifier> logger,
            IBotService botService)
        {
            _logger = logger;
            _botService = botService;
        }

        public async Task Notify(SubscribedUser subscribedUser, DefaultNotifyPayload payload)
        {
            var datesWithChangedSchedule = new HashSet<DateTime>();
            foreach (var lesson in payload.UpdatedLessons) {
                datesWithChangedSchedule.Add(lesson.LessonDate);
            }

            await _botService.Client.SendTextMessageAsync(
                new ChatId(subscribedUser.ChatId), "Schedule was updated");
        }
    }
}