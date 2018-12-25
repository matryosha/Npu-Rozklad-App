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
    public class MessageHandleService
    {
        private readonly RozkladBotContext _context;
        private readonly BotService _bot;
        private readonly ILessonsProvider _lessonsProvider;
        private readonly UnknownResponseConfiguration _replyStickers;
        public MessageHandleService(RozkladBotContext context, 
            BotService botService, ILessonsProvider lessonsProvider, 
            IOptions<UnknownResponseConfiguration> idkStickers)
        {
            _context = context;
            _bot = botService;
            _lessonsProvider = lessonsProvider;
            _replyStickers = idkStickers.Value;
        }

        public async Task HandleTextMessage(Message message)
        {
            var user = message.From;

            var currentRozkladUser = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == user.Id);
            if (currentRozkladUser == null || currentRozkladUser?.IsDeleted == true)
            {
                if (message.Text != "/start")
                {
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Чтобы начать отправь /start");
                    return;
                }

                if (currentRozkladUser == null)
                {
                    var rozkladUser = new RozkladUser
                    {
                        TelegramId = user.Id,
                        Name = user.Username,
                        LastAction = RozkladUser.LastActionType.WaitForFaculty
                    };
                    await _context.Users.AddAsync(rozkladUser);
                }
                else
                {
                    currentRozkladUser.IsDeleted = false;
                    currentRozkladUser.LastAction = RozkladUser.LastActionType.WaitForFaculty;
                    _context.Entry(currentRozkladUser).Property(u => u.IsDeleted).IsModified = true;
                    _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                }
                
                await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Выбери факультет:", 
                    replyMarkup: MessageHandleHelper.GetFacultiesReplyKeyboardMarkup(_lessonsProvider.GetFaculties()));
                await _context.SaveChangesAsync();
                return;
            }
            else
            {
                if (message.Text == "/reset")
                {
                    currentRozkladUser.IsDeleted = true;
                    currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                    _context.Users.Update(currentRozkladUser);
                    await _context.SaveChangesAsync();
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Чтобы начать отправь /start", replyMarkup: new ReplyKeyboardRemove());
                    return;
                }
            }

            currentRozkladUser.QueryCount++;
            _context.Entry(currentRozkladUser).Property(u => u.QueryCount).IsModified = true;
            await _context.SaveChangesAsync();

            switch (currentRozkladUser.LastAction)
            {
                case RozkladUser.LastActionType.WaitForFaculty:
                {
                    var selectedFaculty = _lessonsProvider.GetFaculties().FirstOrDefault(f => f.FullName == message.Text);
                    var faculties = _lessonsProvider.GetFaculties();
                    if (selectedFaculty == null)
                    {
                        await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Выбери факультет:",
                            replyMarkup: MessageHandleHelper.GetFacultiesReplyKeyboardMarkup(faculties));
                        return;
                    }

                    var groupsKeyboard =  MessageHandleHelper.GetGroupsReplyKeyboardMarkup(
                        await _lessonsProvider.GetGroups(selectedFaculty.ShortName));
                    if (!groupsKeyboard.Keyboard.Any())
                    {
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Для этого фака нет групп :(",
                            replyMarkup: MessageHandleHelper.GetFacultiesReplyKeyboardMarkup(faculties));
                        return;
                    }

                    currentRozkladUser.LastAction = RozkladUser.LastActionType.WaitForGroup;
                    currentRozkladUser.FacultyShortName = selectedFaculty.ShortName;
                    _context.Users.Update(currentRozkladUser);
                    await _context.SaveChangesAsync();
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Выбери группу:",
                        replyMarkup: groupsKeyboard);
                    return;
                }
                case RozkladUser.LastActionType.WaitForGroup:
                {
                    var groups = await _lessonsProvider.GetGroups(currentRozkladUser.FacultyShortName);
                    var selectedGroup = groups.FirstOrDefault(g => g.ShortName == message.Text);
                    if (selectedGroup == null)
                    {
                        await _bot.Client.SendTextMessageAsync(message.Chat.Id, "?. Выбери группу:",
                            replyMarkup: MessageHandleHelper.GetGroupsReplyKeyboardMarkup(
                                await _lessonsProvider.GetGroups(currentRozkladUser.FacultyShortName)));
                        return;
                    }

                    currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                    currentRozkladUser.ExternalGroupId = selectedGroup.ExternalId;

                    _context.Users.Update(currentRozkladUser);
                    await _context.SaveChangesAsync();

                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Готово. Выбери действие:",
                        replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());

                    return;
                }
                case RozkladUser.LastActionType.Settings:
                {
                    switch (message.Text)
                    {
                        case "Сбросить настройки":
                        {
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.SettingsReset;
                            _context.Users.Update(currentRozkladUser);
                            await _context.SaveChangesAsync();

                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Точно-точно?",
                                replyMarkup: MessageHandleHelper.GetConfirmButtonsReplyKeyboardMarkup());
                            return;
                        }
                        case "Назад к меню":
                        {
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "#_#",
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
                            _context.Users.Remove(currentRozkladUser);
                            await _context.SaveChangesAsync();
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Done",
                                replyMarkup: new ReplyKeyboardRemove());
                            return;
                        }
                        case "Нет":
                        {
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                            _context.Users.Update(currentRozkladUser);
                            await _context.SaveChangesAsync();
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "=3",
                                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                            return;
                        }
                        default:
                        {
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Сбросить, м?",
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
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                                    await PrintWeekLessonsAsync(
                                        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1), 
                                        currentRozkladUser, message);
                            return;
                        }
                        case "На следующую неделю":
                        {
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                                    await PrintWeekLessonsAsync(
                                        DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1).AddDays(7), 
                                        currentRozkladUser, message);
                            return;
                        }
                        case "Назад к меню":
                        {
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, ":O",
                                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                            return;
                        }
                    }
                    return;
                }
            }
            

            switch (message.Text.Trim())
            {
                case "Расписание":
                {
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "На когда?",
                        replyMarkup: MessageHandleHelper.GetScheduleActionsReplyKeyboardMarkup());
                    return;
                }
                case "Расписание на сегодня":
                {
                    await PrintOneDayLessonsAsync(DateTime.Today, currentRozkladUser, message);
                    return;
                }
                case "Расписание на завтра":
                {
                    await PrintOneDayLessonsAsync(DateTime.Today.AddDays(1), currentRozkladUser, message);
                    return;
                }
                case "Расписание на неделю":
                {
                    currentRozkladUser.LastAction = RozkladUser.LastActionType.WeekSchedule;
                    _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                    await _context.SaveChangesAsync();

                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "На какую неделю?",
                        replyMarkup: MessageHandleHelper.GetWeekScheduleActionsReplyKeyboardMarkup());

                    return;
                }
                case "Назад к меню":
                {
                    if (currentRozkladUser.LastAction != RozkladUser.LastActionType.Default)
                    {
                        currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                        _context.Entry(currentRozkladUser).Property(p => p.LastAction).IsModified = true;
                        await _context.SaveChangesAsync();
                    }
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "парам-пам",
                        replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                    return;
                }
                case "Настройки":
                {
                    currentRozkladUser.LastAction = RozkladUser.LastActionType.Settings;
                    _context.Users.Update(currentRozkladUser);
                    await _context.SaveChangesAsync();
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, ".0.",
                        replyMarkup: MessageHandleHelper.GetSettingsActionsReplyKeyboardMarkup());
                        return;
                }
                default:
                {
                    await _bot.Client.SendStickerAsync(message.Chat.Id, _replyStickers.GetRandomStickerString());
                    return;
                }    
            }
        }

        private async Task PrintWeekLessonsAsync(DateTime startWeekDate, RozkladUser user, Message userMessage)
        {
            await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id,
                $"Пары на неделю:");

            await _bot.Client.SendChatActionAsync(userMessage.Chat.Id, ChatAction.Typing);

            for (int dayNumber = 0; dayNumber < 5; dayNumber++)
            {
                var currentDate = startWeekDate.AddDays(dayNumber);
                var lessons = await _lessonsProvider.GetLessonsOnDate(user.FacultyShortName, user.ExternalGroupId, currentDate);
                if (!lessons.Any())
                {
                    await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id, 
                        $@"Пар на {MessageHandleHelper.ConvertDayOfWeekToText(currentDate.DayOfWeek)} {currentDate:dd/MM} нету :)", 
                        replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
                    continue;
                }

                await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id,
                    MessageHandleHelper.CreateOneDayWeekLessonsMessage(lessons, currentDate), parseMode:ParseMode.Markdown);
            }

            await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id, "жж", 
                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
        }

        private async Task PrintOneDayLessonsAsync(DateTime time, RozkladUser user, Message userMessage)
        {
            await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id,
                $"Пары на *{MessageHandleHelper.ConvertDayOfWeekToText(time.DayOfWeek)}* `{time:dd/MM}`", ParseMode.Markdown);
            await _bot.Client.SendChatActionAsync(userMessage.Chat.Id, ChatAction.Typing);
            var lessons = await _lessonsProvider.GetLessonsOnDate(user.FacultyShortName, user.ExternalGroupId, time);
            if (!lessons.Any())
            {
                await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id, "Пар нет :)", 
                    replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
            }
            foreach (var lesson in lessons)
            {
                await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id,
                    MessageHandleHelper.CreateOneDayLessonMessage(lesson), ParseMode.Markdown);
            }
            await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id, "пум-пум", 
                replyMarkup: MessageHandleHelper.GetMainMenuReplyKeyboardMarkup());
        }
    }
}
