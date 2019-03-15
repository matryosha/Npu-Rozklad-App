using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RozkladNpuAspNetCore.Helpers;
using RozkladNpuAspNetCore.Interfaces;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Services
{
    public class KeyboardReplyService : IKeyboardReplyService
    {
        private readonly BotService _botService;

        public KeyboardReplyService(
            BotService botService)
        {
            _botService = botService;
        }

        public Task ShowMainMenu(Message message)
        {
            return _botService.Client.SendTextMessageAsync(message.Chat.Id, "Choose action: ",
                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
        }

        public Task ShowScheduleMenu(Message message)
        {
            return _botService.Client.SendTextMessageAsync(message.Chat.Id, "Groups: ",
                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
        }
    }
}
