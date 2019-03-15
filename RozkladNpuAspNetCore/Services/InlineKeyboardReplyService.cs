using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RozkladNpuAspNetCore.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RozkladNpuAspNetCore.Services
{
    public class InlineKeyboardReplyService : IInlineKeyboardReplyService
    {
        private readonly BotService _botService;

        public InlineKeyboardReplyService(
            BotService botService)
        {
            _botService = botService;
        }

        public Task ShowScheduleMenu(Message message)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = message.From.Id + ';' + "Add group action",
                        Text = "Add group"
                    }
                }
            });
            return _botService.Client.SendTextMessageAsync(
                message.Chat.Id,
                "Choose group:",
                replyMarkup: inlineKeyboard);
        }
    }
}
