using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Entities;
using RozkladNpuAspNetCore.Helpers;
using RozkladNpuAspNetCore.Infrastructure;
using RozkladNpuAspNetCore.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RozkladNpuAspNetCore.Services
{
    public class InlineKeyboardReplyService : IInlineKeyboardReplyService
    {
        private readonly BotService _botService;
        private readonly ILessonsProvider _lessonsProvider;
        private readonly IUserService _userService;

        public InlineKeyboardReplyService(
            BotService botService,
            ILessonsProvider lessonsProvider,
            IUserService userService)
        {
            _botService = botService;
            _lessonsProvider = lessonsProvider;
            _userService = userService;
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
                        Text = "Add group"
                    }
                });

                if (!spawnNewMenu && 
                    _userService.TryGetLastMessageId(message.Chat.Id, out int messageId))
                {
                    return await _botService.Client.EditMessageTextAsync(
                        message.Chat.Id,
                        messageId,
                        text: "Choose group",
                        replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
                }
                else
                {
                    var sentMessage = await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose group:",
                        replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
                    _userService.SetLastMessageId(message.Chat.Id, sentMessage.MessageId);
                    return sentMessage;
                }

            }
        }
        
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
                MessageHandleHelper.OneDayClassesMessage(
                    classes, 
                    classesDate,
                    group);

            var inlineReplyKeyboard = new List<List<InlineKeyboardButton>>();
            var weekButtonsRow = new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Monday ? "*Mo" : "Mo",
                    CallbackData = 
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Monday, userTelegramId)
                },
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Tuesday ? "*Tu" : "Tu",
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Tuesday, userTelegramId)
                },
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Wednesday ? "*Wd" :"Wd",
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Wednesday, userTelegramId)
                },
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Thursday ? "*Th" : "Th",
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Thursday, userTelegramId)
                },
                new InlineKeyboardButton
                {
                    Text = dayOfWeek == DayOfWeek.Friday ? "*Fr" : "Fr",
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, week, DayOfWeek.Friday, userTelegramId)
                }
            };
            var weekSelectButtonsRow = new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton
                {
                    Text = week == ShowGroupSelectedWeek.ThisWeek ? "*This week" : "This week",
                    CallbackData =
                        CallbackQueryDataConverters.GetGroupScheduleCallbackData(
                            group, ShowGroupSelectedWeek.ThisWeek, dayOfWeek, userTelegramId)

                },
                new InlineKeyboardButton
                {
                    Text = week == ShowGroupSelectedWeek.NextWeek ? "*Next week" : "Next week",
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
                    Text = "Add group",
                    CallbackData = ((int)CallbackQueryType.AddGroup).ToString()
                });
            }
            else
            {
                actionButtonsRow.Add(new InlineKeyboardButton
                {
                    Text = "Back",
                    CallbackData = ((int)CallbackQueryType.ShowScheduleMenu).ToString()
                });
            }
            actionButtonsRow.Add(new InlineKeyboardButton
            {
                Text = "Delete group",
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

    }
}
