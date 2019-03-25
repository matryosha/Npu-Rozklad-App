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
        private readonly IBotService _botService;
        private readonly ILessonsProvider _lessonsProvider;
        private readonly ILocalizationService _localization;

        public KeyboardReplyService(
            IBotService botService,
            ILessonsProvider lessonsProvider,
            ILocalizationService localization)
        {
            _botService = botService;
            _lessonsProvider = lessonsProvider;
            _localization = localization;
        }

        public Task<Message> ShowMainMenu(Message message)
        {
            return _botService.Client.SendTextMessageAsync(
                message.Chat.Id, _localization["ua", "choose-action-message"],
                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
        }

        public Task<Message> ShowFacultyList(Message message)
        {
            return _botService.Client.SendTextMessageAsync(
                message.Chat.Id,
                _localization["ua", "choose-faculty-message"],
                replyMarkup: MessageHandleHelper.GetFacultiesReplyKeyboardMarkup(
                    _lessonsProvider.GetFaculties()));
        }

        public async Task<Message> ShowGroupList(Message message, Faculty faculty)
        {
            var groupsKeyboard =  MessageHandleHelper.GetGroupsReplyKeyboardMarkup(
                await _lessonsProvider.GetGroups(faculty.ShortName));

            if (!groupsKeyboard.Keyboard.Any())
            {
                return await _botService.Client.SendTextMessageAsync(
                    message.Chat.Id,
                    _localization["ua", "no-groups-for-faculty-message"]);
            }
            
            return await _botService.Client.SendTextMessageAsync(
                message.Chat.Id,
                _localization["ua", "choose-group-message"],
                replyMarkup: groupsKeyboard);
        }
    }
}
