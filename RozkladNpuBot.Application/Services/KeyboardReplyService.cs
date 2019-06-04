using System.Linq;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuBot.Application.Interfaces;
using RozkladNpuBot.Infrastructure;
using RozkladNpuBot.Infrastructure.Interfaces;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Application.Services
{
    public class KeyboardReplyService : IKeyboardReplyService
    {
        private readonly IBotService _botService;
        private readonly ILessonsProvider _lessonsProvider;
        private readonly ILocalizationService _localization;
        private readonly ReplyKeyboardMarkupCreator _replyKeyboardMarkupCreator;

        public KeyboardReplyService(
            IBotService botService,
            ILessonsProvider lessonsProvider,
            ILocalizationService localization,
            ReplyKeyboardMarkupCreator replyKeyboardMarkupCreator)
        {
            _botService = botService;
            _lessonsProvider = lessonsProvider;
            _localization = localization;
            _replyKeyboardMarkupCreator = replyKeyboardMarkupCreator;
        }

        public Task<Message> ShowMainMenu(Message message)
        {
            return _botService.Client.SendTextMessageAsync(
                message.Chat.Id, _localization["ua", "choose-action-message"],
                replyMarkup: _replyKeyboardMarkupCreator.MainMenuMarkup());
        }

        public Task<Message> ShowFacultyList(Message message)
        {
            return _botService.Client.SendTextMessageAsync(
                message.Chat.Id,
                _localization["ua", "choose-faculty-message"],
                replyMarkup: _replyKeyboardMarkupCreator.FacultiesMarkup(
                    _lessonsProvider.GetFaculties()));
        }

        public async Task<Message> ShowGroupList(Message message, Faculty faculty)
        {
            var groupsKeyboard = _replyKeyboardMarkupCreator.GroupsMarkup(
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
