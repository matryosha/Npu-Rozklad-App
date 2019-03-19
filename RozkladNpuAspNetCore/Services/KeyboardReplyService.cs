using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Helpers;
using RozkladNpuAspNetCore.Interfaces;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Services
{
    public class KeyboardReplyService : IKeyboardReplyService
    {
        private readonly BotService _botService;
        private readonly ILessonsProvider _lessonsProvider;

        public KeyboardReplyService(
            BotService botService,
            ILessonsProvider lessonsProvider)
        {
            _botService = botService;
            _lessonsProvider = lessonsProvider;
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

        public Task ShowFacultyList(Message message)
        {
            return _botService.Client.SendTextMessageAsync(message.Chat.Id, "Choose faculty: ",
                    replyMarkup: MessageHandleHelper.GetFacultiesReplyKeyboardMarkup(_lessonsProvider.GetFaculties()));
        }

        public async Task<bool> ShowGroupList(Message message, Faculty faculty)
        {
            var groupsKeyboard =  MessageHandleHelper.GetGroupsReplyKeyboardMarkup(
                await _lessonsProvider.GetGroups(faculty.ShortName));

            if (!groupsKeyboard.Keyboard.Any())
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "There are no groups for this faculty");
                return false;
            }
            
            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Choose group: ",
                replyMarkup: groupsKeyboard);
            return true;
        }
    }
}
