using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RozkladNpuBot.Application.Configurations;
using RozkladNpuBot.Application.Interfaces;
using RozkladNpuBot.Domain.Entities;
using RozkladNpuBot.Infrastructure;
using RozkladNpuBot.Infrastructure.Interfaces;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Application.Services
{
    public class MessageHandleService : IMessageHandleService
    {
        private readonly IBotService _botService;
        private readonly ILessonsProvider _lessonsProvider;
        private readonly IUserService _userService;
        private readonly IKeyboardReplyService _keyboardReplyService;
        private readonly IInlineKeyboardReplyService _inlineKeyboardReplyService;
        private readonly ILocalizationService _localization;
        private readonly ReplyKeyboardMarkupCreator _replyKeyboardMarkupCreator;
        private readonly ILogger<MessageHandleService> _logger;
        private readonly UnknownResponseConfiguration _replyStickers;
        public MessageHandleService(
            IBotService botService, 
            ILessonsProvider lessonsProvider, 
            IOptions<UnknownResponseConfiguration> idkStickers,
            IUserService userService,
            IKeyboardReplyService keyboardReplyService,
            IInlineKeyboardReplyService inlineKeyboardReplyService,
            ILocalizationService localization,
            ReplyKeyboardMarkupCreator replyKeyboardMarkupCreator,
            ILogger<MessageHandleService> logger)
        {
            _botService = botService;
            _lessonsProvider = lessonsProvider;
            _userService = userService;
            _keyboardReplyService = keyboardReplyService;
            _inlineKeyboardReplyService = inlineKeyboardReplyService;
            _localization = localization;
            _replyKeyboardMarkupCreator = replyKeyboardMarkupCreator;
            _logger = logger;
            _replyStickers = idkStickers.Value;
        }

        public async Task HandleMessage(Message message)
        {
            var sender = message.From;
            var user = await _userService.GetUser(sender.Id);
            if (user == null || user.IsDeleted)
            {               
                if (user == null)
                {
                    var newUser = new RozkladUser
                    {
                        TelegramId = sender.Id,
                        Name = sender.Username
                    };
                    await _userService.AddUser(newUser);
                }
                else
                {
                    user.IsDeleted = false;
                    await _userService.UpdateUser(user);
                }

                await _keyboardReplyService.ShowMainMenu(message);
                return;
            }
            else if (message.Text == "/reset")
            {
                
                user.IsDeleted = true;
                user.Groups.Clear();
                await _userService.UpdateUser(user);
                return;
            }

            user.QueryCount++;
            await _userService.UpdateUser(user);

            switch (user.LastAction)
            {
                case RozkladUser.LastActionType.WaitForFaculty:
                {
                    var selectedFaculty = _lessonsProvider.GetFaculties().FirstOrDefault(f => f.FullName == message.Text);
                    if (selectedFaculty == null)
                    {
                        await _keyboardReplyService.ShowMainMenu(message);
                        break;
                    }

                    var showGroupListMessage = await _keyboardReplyService.ShowGroupList(message, selectedFaculty);
                    if (showGroupListMessage.Text == _localization["ua", "choose-group-message"])
                    {
                        user.LastAction = RozkladUser.LastActionType.WaitForGroup;
                        user.FacultyShortName = selectedFaculty.ShortName;
                        await _userService.UpdateUser(user);
                    }

                    return;
                }
                case RozkladUser.LastActionType.WaitForGroup:
                {
                    var groups = await _lessonsProvider.GetGroups(user.FacultyShortName);
                    var selectedGroup = groups.FirstOrDefault(g => g.ShortName == message.Text);
                    if (selectedGroup == null)
                    {
                        await _keyboardReplyService.ShowMainMenu(message);
                        break;
                    }

                    user.LastAction = RozkladUser.LastActionType.Default;
                    user.FacultyShortName = String.Empty;
                    user.Groups.Add(selectedGroup);

                    await _userService.UpdateUser(user);

                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, _localization["ua", "choose-action-message"],
                        replyMarkup: _replyKeyboardMarkupCreator.MainMenuMarkup());

                    return;
                }
            }

            if (message.Text == _localization["ua", "schedule-reply-keyboard"])
            {
                await _inlineKeyboardReplyService.ShowScheduleMenu(
                    message,
                    user,
                    spawnNewMenu: true);
                return;
            }
            else if (message.Text == _localization["ua", "menu-reply-keyboard"])
            {
                user.LastAction = RozkladUser.LastActionType.Default;
                await _userService.UpdateUser(user);
                await _keyboardReplyService.ShowMainMenu(message);
                return;
            }

            await _botService.Client.SendStickerAsync(message.Chat.Id, _replyStickers.GetRandomStickerString());
            await _keyboardReplyService.ShowMainMenu(message);
        }
    }
}
