using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RozkladNpuAspNetCore.Configurations;
using RozkladNpuAspNetCore.Entities;
using RozkladNpuAspNetCore.Helpers;
using RozkladNpuAspNetCore.Interfaces;
using RozkladNpuAspNetCore.Persistence;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RozkladNpuAspNetCore.Services
{
    public class MessageHandleService : IMessageHandleService
    {
//        private readonly RozkladBotContext _context;
        private readonly BotService _botService;
        private readonly ILessonsProvider _lessonsProvider;
        private readonly IUserService _userService;
        private readonly IKeyboardReplyService _keyboardReplyService;
        private readonly IInlineKeyboardReplyService _inlineKeyboardReplyService;
        private readonly UnknownResponseConfiguration _replyStickers;
        public MessageHandleService(RozkladBotContext context, 
            BotService botService, ILessonsProvider lessonsProvider, 
            IOptions<UnknownResponseConfiguration> idkStickers,
            IUserService userService,
            IKeyboardReplyService keyboardReplyService,
            IInlineKeyboardReplyService inlineKeyboardReplyService)
        {
//            _context = context;
            _botService = botService;
            _lessonsProvider = lessonsProvider;
            _userService = userService;
            _keyboardReplyService = keyboardReplyService;
            _inlineKeyboardReplyService = inlineKeyboardReplyService;
            _replyStickers = idkStickers.Value;
        }

        public async Task HandleMessage(Message message)
        {
            var sender = message.From;
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == sender.Id);
            var user = await _userService.GetUser(sender.Id);
            if (user == null || user.IsDeleted)
            {
//                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Choose action:");
                
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

//                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Выбери факультет:", 
//                    replyMarkup: MessageHandleHelper.GetFacultiesReplyKeyboardMarkup(_lessonsProvider.GetFaculties()));
//                await _context.SaveChangesAsync();
                return;
            }
            else if (message.Text == "/reset")
            {
                user.IsDeleted = true;
                await _userService.UpdateUser(user);
                return;
            }

//            user.QueryCount++;
//            await _userService.UpdateUser(user);

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

                    if (await _keyboardReplyService.ShowGroupList(message, selectedFaculty))
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

                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Готово. Выбери действие:",
                        replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());

                    return;
                }
            }

            switch (message.Text)
            {
                case "Schedule":
                {
                    await _inlineKeyboardReplyService.ShowScheduleMenu(message, user);
                    return;

                    /*var selectedFaculty = _lessonsProvider.GetFaculties().FirstOrDefault(f => f.FullName == message.Text);
                    var faculties = _lessonsProvider.GetFaculties();
                    if (selectedFaculty == null)
                    {
                        await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Выбери факультет:",
                            replyMarkup: MessageHandleHelper.GetFacultiesReplyKeyboardMarkup(faculties));
                        return;
                    }*/

/*                    var groupsKeyboard =  MessageHandleHelper.GetGroupsReplyKeyboardMarkup(
                        await _lessonsProvider.GetGroups(selectedFaculty.ShortName));
                    if (!groupsKeyboard.Keyboard.Any())
                    {
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Для этого фака нет групп :(",
                            replyMarkup: MessageHandleHelper.GetFacultiesReplyKeyboardMarkup(faculties));
                        return;
                    }

                    user.LastAction = RozkladUser.LastActionType.WaitForGroup;
                    user.FacultyShortName = selectedFaculty.ShortName;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Выбери группу:",
                        replyMarkup: groupsKeyboard);
                    return;*/
                }
                case "Menu":
                {
                    user.LastAction = RozkladUser.LastActionType.Default;
                    await _userService.UpdateUser(user);
                    await _keyboardReplyService.ShowMainMenu(message);
                    return;
                }
                
                /*case RozkladUser.LastActionType.Settings:
                {
                    switch (message.Text)
                    {
                        case "Сбросить настройки":
                        {
                            user.LastAction = RozkladUser.LastActionType.SettingsReset;
                            _context.Users.Update(user);
                            await _context.SaveChangesAsync();

                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Точно-точно?",
                                replyMarkup: MessageHandleHelper.GetConfirmButtonsReplyKeyboardMarkup());
                            return;
                        }
                        case "Назад к меню":
                        {
                            user.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(user).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "#_#",
                                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                           return;
                        }
                    }

                    return;
                }
                case RozkladUser.LastActionType.SettingsReset:
                {
                    switch (message.Text)
                    {
                        case "Да":
                        {
                            _context.Users.Remove(user);
                            await _context.SaveChangesAsync();
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Done",
                                replyMarkup: new ReplyKeyboardRemove());
                            return;
                        }
                        case "Нет":
                        {
                            user.LastAction = RozkladUser.LastActionType.Default;
                            _context.Users.Update(user);
                            await _context.SaveChangesAsync();
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "=3",
                                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                            return;
                        }
                        default:
                        {
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Сбросить, м?",
                                replyMarkup: MessageHandleHelper.GetConfirmButtonsReplyKeyboardMarkup());
                            return;
                        }
                    }
                }
                case RozkladUser.LastActionType.WeekSchedule:
                {
                    switch (message.Text)
                    {
                        case "На текущую неделю":
                        {
                            user.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(user).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                                    await PrintWeekLessonsAsync(
                                        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1), 
                                        user, message);
                            return;
                        }
                        case "На следующую неделю":
                        {
                            user.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(user).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                                    await PrintWeekLessonsAsync(
                                        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1).AddDays(7), 
                                        user, message);
                            return;
                        }
                        case "Назад к меню":
                        {
                            user.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(user).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                            await _botService.Client.SendTextMessageAsync(message.Chat.Id, ":O",
                                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                            return;
                        }
                    }
                    return;
                }*/
            }


            /*switch (message.Text.Trim())
            {
                case "Расписание":
                {
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "На когда?",
                        replyMarkup: MessageHandleHelper.GetScheduleActionsReplyKeyboardMarkup());
                    return;
                }
                case "Расписание на сегодня":
                {
                    await PrintOneDayLessonsAsync(DateTime.Today, user, message);
                    return;
                }
                case "Расписание на завтра":
                {
                    await PrintOneDayLessonsAsync(DateTime.Today.AddDays(1), user, message);
                    return;
                }
                case "Расписание на неделю":
                {
                    user.LastAction = RozkladUser.LastActionType.WeekSchedule;
                    _context.Entry(user).Property(u => u.LastAction).IsModified = true;
                    await _context.SaveChangesAsync();

                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "На какую неделю?",
                        replyMarkup: MessageHandleHelper.GetWeekScheduleActionsReplyKeyboardMarkup());

                    return;
                }
                case "Назад к меню":
                {
                    if (user.LastAction != RozkladUser.LastActionType.Default)
                    {
                        user.LastAction = RozkladUser.LastActionType.Default;
                        _context.Entry(user).Property(p => p.LastAction).IsModified = true;
                        await _context.SaveChangesAsync();
                    }
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, "парам-пам",
                        replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                    return;
                }
                case "Настройки":
                {
                    user.LastAction = RozkladUser.LastActionType.Settings;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    await _botService.Client.SendTextMessageAsync(message.Chat.Id, ".0.",
                        replyMarkup: MessageHandleHelper.GetSettingsActionsReplyKeyboardMarkup());
                        return;
                }
                default:
                {
                    await _botService.Client.SendStickerAsync(message.Chat.Id, _replyStickers.GetRandomStickerString());
                    return;
                }    
            }*/
            await _keyboardReplyService.ShowMainMenu(message);
        }

        private async Task PrintWeekLessonsAsync(DateTime startWeekDate, RozkladUser user, Message userMessage)
        {
            await _botService.Client.SendTextMessageAsync(userMessage.Chat.Id,
                $"Пары на неделю:");

            await _botService.Client.SendChatActionAsync(userMessage.Chat.Id, ChatAction.Typing);

            for (int dayNumber = 0; dayNumber < 5; dayNumber++)
            {
                var currentDate = startWeekDate.AddDays(dayNumber);
                var lessons = await _lessonsProvider.GetLessonsOnDate(user.FacultyShortName, user.ExternalGroupId, currentDate);
                if (!lessons.Any())
                {
                    await _botService.Client.SendTextMessageAsync(userMessage.Chat.Id, 
                        $@"Пар на {MessageHandleHelper.ConvertDayOfWeekToText(currentDate.DayOfWeek)} {currentDate:dd/MM} нету :)", 
                        replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                    continue;
                }

                await _botService.Client.SendTextMessageAsync(userMessage.Chat.Id,
                    MessageHandleHelper.OneDayClassesMessage(lessons, currentDate), parseMode:ParseMode.Markdown);
            }

            await _botService.Client.SendTextMessageAsync(userMessage.Chat.Id, "жж", 
                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
        }

        private async Task PrintOneDayLessonsAsync(DateTime time, RozkladUser user, Message userMessage)
        {
            await _botService.Client.SendTextMessageAsync(userMessage.Chat.Id,
                $"Пары на *{MessageHandleHelper.ConvertDayOfWeekToText(time.DayOfWeek)}* `{time:dd/MM}`", ParseMode.Markdown);
            await _botService.Client.SendChatActionAsync(userMessage.Chat.Id, ChatAction.Typing);
            var lessons = await _lessonsProvider.GetLessonsOnDate(user.FacultyShortName, user.ExternalGroupId, time);
            if (!lessons.Any())
            {
                await _botService.Client.SendTextMessageAsync(userMessage.Chat.Id, "Пар нет :)", 
                    replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
            }
            foreach (var lesson in lessons)
            {
                await _botService.Client.SendTextMessageAsync(userMessage.Chat.Id,
                    MessageHandleHelper.CreateOneDayLessonMessage(lesson), ParseMode.Markdown);
            }
            await _botService.Client.SendTextMessageAsync(userMessage.Chat.Id, "пум-пум", 
                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
        }

        public Task ShowMainMenu()
        {
            throw new NotImplementedException();
        }

        public Task ShowScheduleMenu()
        {
            throw new NotImplementedException();
        }
    }
}
