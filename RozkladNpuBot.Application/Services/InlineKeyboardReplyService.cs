using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuBot.Application.Enums;
using RozkladNpuBot.Application.Extensions;
using RozkladNpuBot.Application.Helpers;
using RozkladNpuBot.Application.Interfaces;
using RozkladNpuBot.Domain.Entities;
using RozkladNpuBot.Infrastructure;
using RozkladNpuBot.Infrastructure.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RozkladNpuBot.Application.Services
{
    public class InlineKeyboardReplyService : IInlineKeyboardReplyService
    {
        private readonly IBotService _botService;
        private readonly ILessonsProvider _lessonsProvider;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localization;
        private readonly MessageBuilderService _messageBuilderService;

        public InlineKeyboardReplyService(
            IBotService botService,
            ILessonsProvider lessonsProvider,
            IUserService userService,
            ILocalizationService localization,
            MessageBuilderService messageBuilderService)
        {
            _botService = botService;
            _lessonsProvider = lessonsProvider;
            _userService = userService;
            _localization = localization;
            _messageBuilderService = messageBuilderService;
        }


        public async Task<Message> ShowScheduleMenu(Message message, int telegramId)
        {
            var user = await _userService.GetUser(telegramId);
            return await ShowScheduleMenu(message, user);
        }
        
        public async Task<Message> ShowScheduleMenu(
            Message message, 
            RozkladUser user,
            bool spawnNewMenu = false)
        {
//            await _botService.Client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            var inlineKeyboard = new List<List<InlineKeyboardButton>>();
            var rowButtons = new List<InlineKeyboardButton>();
            if (user.Groups.Count == 1)
            {
                return await ShowGroupMenu(
                    message,
                    user.Groups.FirstOrDefault(),
                    DateTime.Today.ToLocal().DayOfWeek,
                    ShowGroupSelectedWeek.ThisWeek,
                    user.TelegramId,
                    isSingleGroup: true,
                    spawnNewMenu: spawnNewMenu);
            }
            else
            {
                foreach (var group in user.Groups)
                {
                    var button = new InlineKeyboardButton
                    {
                        CallbackData =
                            CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                                group, ShowGroupSelectedWeek.ThisWeek, user.TelegramId),
                        Text = @group.ShortName
                    };
                    if (rowButtons.Count < 2)
                    {
                        rowButtons.Add(button);
                    }
                    else
                    {
                        inlineKeyboard.Add(rowButtons);
                        rowButtons = new List<InlineKeyboardButton>();
                        rowButtons.Add(button);
                    }
                }

                inlineKeyboard.Add(rowButtons);
                inlineKeyboard.Add(new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton
                    {
                        CallbackData = ((int) CallbackQueryType.AddGroup).ToString(),
                        Text = _localization["ua", "add-group"]
                    }
                });

                if (!spawnNewMenu && 
                    _userService.TryGetLastMessageId(message.Chat.Id, out int messageId))
                {
                    return await _botService.Client.EditMessageTextAsync(
                        message.Chat.Id,
                        messageId,
                        text: _localization["ua", "choose-group-message"],
                        replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
                }
                else
                {
                    var sentMessage = await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        _localization["ua", "choose-group-message"],
                        replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
                    _userService.SetLastMessageId(message.Chat.Id, sentMessage.MessageId);
                    return sentMessage;
                }

            }
        }
        //todo extract isSingleGroup
        /// <param name="spawnNewMenu">Print group menu rather than editing last</param>
        public async Task<Message> ShowGroupMenu(
            Message callbackQueryMessage, 
            Group @group, 
            DayOfWeek dayOfWeek, 
            ShowGroupSelectedWeek week,
            int userTelegramId,
            bool isSingleGroup,
            bool spawnNewMenu = false)
        {
            DateTime classesDate;
            var currentDay = DateTime.Today.ToLocal().DayOfWeek;
            classesDate = DateTime.Today.AddDays(dayOfWeek - currentDay).ToLocal();
            if (week == ShowGroupSelectedWeek.NextWeek)
            {
                classesDate = classesDate.AddDays(7);
            }
           
            var classes = await _lessonsProvider.GetLessonsOnDate(
                group.FacultyShortName,
                group.ExternalId,
                classesDate);

            var message =
                _messageBuilderService.OneDayClassesMessage(
                    classes, 
                    classesDate,
                    group);

            var inlineReplyKeyboard = new List<List<InlineKeyboardButton>>();
            var weekButtonsRow = new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Monday
                        ? _localization["ua", "monday-short"].AsActive()
                        : _localization["ua", "monday-short"],
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Monday, userTelegramId)
                },
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Tuesday
                        ? _localization["ua", "tuesday-short"].AsActive()
                        : _localization["ua", "tuesday-short"],
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Tuesday, userTelegramId)
                },
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Wednesday
                        ? _localization["ua", "wednesday-short"].AsActive()
                        : _localization["ua", "wednesday-short"],
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Wednesday, userTelegramId)
                },
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Thursday
                        ? _localization["ua", "thursday-short"].AsActive()
                        : _localization["ua", "thursday-short"],
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Thursday, userTelegramId)
                },
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Friday
                        ? _localization["ua", "friday-short"].AsActive()
                        : _localization["ua", "friday-short"],
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Friday, userTelegramId)
                }
            };
            var weekSelectButtonsRow = new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = week == ShowGroupSelectedWeek.ThisWeek 
                        ? _localization["ua", "this-week"].AsActive()
                        : _localization["ua", "this-week"],
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, ShowGroupSelectedWeek.ThisWeek, dayOfWeek, userTelegramId)

                },
                new InlineKeyboardButton
                {
                    Text = week == ShowGroupSelectedWeek.NextWeek 
                        ? _localization["ua", "next-week"].AsActive()
                        : _localization["ua", "next-week"],
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, ShowGroupSelectedWeek.NextWeek, dayOfWeek, userTelegramId)
                }
            };
            var actionButtonsRow = new List<InlineKeyboardButton>();
            if (isSingleGroup)
            {
                actionButtonsRow.Add(new InlineKeyboardButton
                {
                    Text = _localization["ua", "add-group"],
                    CallbackData = ((int)CallbackQueryType.AddGroup).ToString()
                });
            }
            else
            {
                actionButtonsRow.Add(new InlineKeyboardButton
                {
                    Text = _localization["ua", "back"],
                    CallbackData = ((int)CallbackQueryType.ShowScheduleMenu).ToString()
                });
            }
            actionButtonsRow.Add(new InlineKeyboardButton
            {
                Text = _localization["ua", "delete"],
                CallbackData = (int)CallbackQueryType.DeleteGroup + 
                               $";{userTelegramId};{group.ExternalId}"
            });
            inlineReplyKeyboard.Add(weekButtonsRow);
            inlineReplyKeyboard.Add(weekSelectButtonsRow);
            inlineReplyKeyboard.Add(actionButtonsRow);

            if (!spawnNewMenu &&
                _userService.TryGetLastMessageId(callbackQueryMessage.Chat.Id, out int messageId))
            {
                return await _botService.Client.EditMessageTextAsync(
                    callbackQueryMessage.Chat.Id,
                    messageId,
                    message,
                    replyMarkup: new InlineKeyboardMarkup(inlineReplyKeyboard),
                    parseMode: ParseMode.Markdown);
            }
            else
            {
                var sentMessage = await _botService.Client.SendTextMessageAsync(
                    callbackQueryMessage.Chat.Id,
                    message,
                    replyMarkup: new InlineKeyboardMarkup(inlineReplyKeyboard),
                    parseMode: ParseMode.Markdown);

                _userService.SetLastMessageId(
                    callbackQueryMessage.Chat.Id,
                    sentMessage.MessageId);
                return sentMessage;
            }
            
        }

        public async Task<Message> ShowNotificationMenu(
            Message message, RozkladUser user, bool spawnNewMenu = false)
        {
            var inlineKeyboard = new List<List<InlineKeyboardButton>>();
            var rowButtons = new List<InlineKeyboardButton>();
            
            foreach (var group in user.Groups)
            {
                var button = new InlineKeyboardButton
                {
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupNotificationCallbackData(
                            group, user.TelegramId),
                    Text = group.ShortName
                };
                if (rowButtons.Count < 2)
                {
                    rowButtons.Add(button);
                }
                else
                {
                    inlineKeyboard.Add(rowButtons);
                    rowButtons = new List<InlineKeyboardButton>();
                    rowButtons.Add(button);
                }
            }
            inlineKeyboard.Add(rowButtons);
            
            if (!spawnNewMenu &&
                _userService.TryGetLastMessageId(message.Chat.Id, out int messageId))
            {
                return await _botService.Client.EditMessageTextAsync(
                    message.Chat.Id,
                    messageId,
                    text: _localization["ua", "choose-group-message"],
                    replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
            }
            else
            {
                var sentMessage = await _botService.Client.SendTextMessageAsync(
                    message.Chat.Id,
                    _localization["ua", "choose-group-message"],
                    replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
                _userService.SetLastMessageId(message.Chat.Id, sentMessage.MessageId);
                return sentMessage;
            }
        }

        public Task<Message> ShowNotificationMenuForGroup(Message message, Group group)
        {
            var text = $"{_localization["ua", "notification-for"]} {group.ShortName}";
            var subscribeButton = new InlineKeyboardButton
            {
                Text = _localization["ua", "turn-on"],
                CallbackData = 
                    CallbackQueryDataConverters.GetSwitchingGroupSubscribeStatusCallbackData(
                        CallbackQueryType.SubscribeToScheduleNotification, group)
            };
            var unsubscribeButton = new InlineKeyboardButton
            {
                Text = _localization["ua", "turn-off"],
                CallbackData = 
                    CallbackQueryDataConverters.GetSwitchingGroupSubscribeStatusCallbackData(
                        CallbackQueryType.UnsubscribeFromScheduleNotification, group)
            };
            var inlineKeyboardMarkup = new[]
            {
                subscribeButton,
                unsubscribeButton
            };
            if (_userService.TryGetLastMessageId(message.Chat.Id, out int messageId))
                return _botService.Client.EditMessageTextAsync(
                    message.Chat.Id,
                    messageId,
                    text,
                    replyMarkup: new InlineKeyboardMarkup(inlineKeyboardMarkup));
            return _botService.Client.SendTextMessageAsync(
                message.Chat.Id, text,
                replyMarkup: new InlineKeyboardMarkup(inlineKeyboardMarkup));
        }
    }
}
