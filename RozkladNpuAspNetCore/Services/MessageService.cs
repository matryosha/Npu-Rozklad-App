using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Models;
using RozkladNpuAspNetCore.Utils;
using RozkladNpuBot.Utils;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RozkladNpuAspNetCore.Services
{
    public class MessageService
    {
        private BaseDbContext _context;
        private BotService _bot;
        private NpuParser _parser;
        private IdkStickers _replyStickers;
        public MessageService(BaseDbContext context, BotService botService, NpuParser parser, IOptions<IdkStickers> idkStikers)
        {
            _context = context;
            _bot = botService;
            _parser = parser;
            _replyStickers = idkStikers.Value;
        }

        public async Task TakeMessage(Message message)
        {
            var user = message.From;
            if (message.Sticker != null)
            {
                await _bot.Client.SendTextMessageAsync(message.Chat.Id, message.Sticker.FileId);
            }
            var currentRozkladUser = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == user.Id);
            if (currentRozkladUser == null)
            {
                if (message.Text != "/start")
                {
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Чтобы начать отправь /start");
                    return;
                }

                var rozkladUser = new RozkladUser
                {
                    TelegramId = user.Id,
                    Name = user.Username,
                    LastAction = RozkladUser.LastActionType.WaitForFaculty
                };
                List<List<KeyboardButton>> rows = GetFacultiesKeyBoardRows();

                ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(rows);

                await _context.Users.AddAsync(rozkladUser);
                await _context.SaveChangesAsync();
                await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Выбери факультет:", replyMarkup: keyboard);
                return;
            }
            else
            {
                if (message.Text == "/reset")
                {
                    _context.Users.Remove(currentRozkladUser);
                    await _context.SaveChangesAsync();
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "рип", replyMarkup: new ReplyKeyboardRemove());
                    return;
                }
            }

            switch (currentRozkladUser.LastAction)
            {
                case RozkladUser.LastActionType.WaitForFaculty:
                {
                    var selectedFaculty = _parser.GetFaculties().FirstOrDefault(f => f.FullName == message.Text);
                    if (selectedFaculty == null)
                    {
                        List<List<KeyboardButton>> facultiesKeyBoardRows = GetFacultiesKeyBoardRows();

                        ReplyKeyboardMarkup facultyKeyboard = new ReplyKeyboardMarkup(facultiesKeyBoardRows);
                        await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Выбери факультет:",
                            replyMarkup: facultyKeyboard);
                        return;
                    }

                   

                    ReplyKeyboardMarkup groupsKeyboard = await GetGroupsKeyboard(selectedFaculty.ShortName);
                    if (!groupsKeyboard.Keyboard.Any())
                    {
                        List<List<KeyboardButton>> rows = GetFacultiesKeyBoardRows();
                        ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(rows);
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Для этого фака нет групп :(",
                            replyMarkup: keyboard);
                        return;
                    }

                    currentRozkladUser.LastAction = RozkladUser.LastActionType.WaitForGroup;
                    currentRozkladUser.FacultyShortName = selectedFaculty.ShortName;
                        _context.Users.Update(currentRozkladUser);
                    await _context.SaveChangesAsync();
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, $"Выбери группу:",
                        replyMarkup: groupsKeyboard);
                    return;
                }
                case RozkladUser.LastActionType.WaitForGroup:
                {
                    var groups = await _parser.GetGroups(currentRozkladUser.FacultyShortName);
                    var selectedGroup = groups.FirstOrDefault(g => g.ShortName == message.Text);
                    if (selectedGroup == null)
                    {
                        ReplyKeyboardMarkup groupsKeyboard =
                            await GetGroupsKeyboard(currentRozkladUser.FacultyShortName);
                        await _bot.Client.SendTextMessageAsync(message.Chat.Id, $"?. Выбери группу:",
                            replyMarkup: groupsKeyboard);
                        return;
                    }

                    currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                    currentRozkladUser.ExternalGroupId = selectedGroup.ExternalId;

                    _context.Users.Update(currentRozkladUser);
                    await _context.SaveChangesAsync();

                    ReplyKeyboardMarkup commonActionsKeyboard = GetCommonActionsKeyboard();
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, $"Готово. Что хочешь, а?:",
                        replyMarkup: commonActionsKeyboard);


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
                                replyMarkup: GetConfirmsButtonsKeyboard());
                            return;
                        }
                        case "Назад к меню":
                        {
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "#_#",
                                replyMarkup: GetCommonActionsKeyboard());
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
                                replyMarkup: GetCommonActionsKeyboard());
                            return;
                        }
                        default:
                        {
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, "Сбросить, м?",
                                replyMarkup: GetConfirmsButtonsKeyboard());
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
                                    await PrintWeekLessons(DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1), currentRozkladUser, message);
                            return;
                        }
                        case "На следующую неделю":
                        {
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                                    await PrintWeekLessons(DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1).AddDays(7), currentRozkladUser, message);
                            return;
                        }
                        case "Назад к меню":
                        {
                            currentRozkladUser.LastAction = RozkladUser.LastActionType.Default;
                            _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                            await _context.SaveChangesAsync();
                            await _bot.Client.SendTextMessageAsync(message.Chat.Id, ":O",
                                replyMarkup: GetCommonActionsKeyboard());
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
                        replyMarkup: GetScheduleActionsKeyboard());
                    return;
                }
                case "Расписание на сегодня":
                {
                    await PrintOneDayLessons(DateTime.Today, currentRozkladUser, message);
                    return;
                }
                case "Расписание на завтра":
                {
                    await PrintOneDayLessons(DateTime.Today.AddDays(1), currentRozkladUser, message);
                    return;
                }
                case "Расписание на неделю":
                {
                    currentRozkladUser.LastAction = RozkladUser.LastActionType.WeekSchedule;
                    _context.Entry(currentRozkladUser).Property(u => u.LastAction).IsModified = true;
                    await _context.SaveChangesAsync();

                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, "На какую неделю?",
                        replyMarkup: GetWeekScheduleActionsKeyboard());

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
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, $"парам-пам",
                        replyMarkup: GetCommonActionsKeyboard());
                    return;
                }
                case "Настройки":
                {
                    currentRozkladUser.LastAction = RozkladUser.LastActionType.Settings;
                    _context.Users.Update(currentRozkladUser);
                    await _context.SaveChangesAsync();
                    await _bot.Client.SendTextMessageAsync(message.Chat.Id, $".0.",
                        replyMarkup: GetSettingsActionsKeyboard());
                        return;
                }
                default:
                {
                    await _bot.Client.SendStickerAsync(message.Chat.Id, _replyStickers.GetRandomStickerString());
                    return;
                }
                    
            }
        }

        private async Task PrintWeekLessons(DateTime startWeekDate, RozkladUser user, Message userMessage)
        {
            await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id,
                $"Пары на неделю:");

            await _bot.Client.SendChatActionAsync(userMessage.Chat.Id, ChatAction.Typing);

            for (int dayNumber = 0; dayNumber < 5; dayNumber++)
            {
                var currentDate = startWeekDate.AddDays(dayNumber);
                var lessons = await _parser.GetLessonsOnDate(user.FacultyShortName, user.ExternalGroupId, currentDate);
                if (!lessons.Any())
                {
                    await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id, 
                        $@"Пар на {LessonsUtils.ConvertDayOfWeekToText(currentDate.DayOfWeek)} {currentDate:dd/MM} нету :)", 
                        replyMarkup: GetCommonActionsKeyboard());
                    continue;
                }

                var oneDayMessage = $@"Пары на {LessonsUtils.ConvertDayOfWeekToText(currentDate.DayOfWeek)} {currentDate:dd/MM}:
";

                foreach (var lesson in lessons)
                {
                    var subgroupInfo = lesson.SubGroup == SubGroup.First ? "1 группа" :
                        lesson.SubGroup == SubGroup.Second ? "2 группа" : "";
                    oneDayMessage += $@"
{lesson.LessonNumber} пара:
{lesson.Subject.Name}
{lesson.Lecturer.FullName ?? ""}
{lesson.Classroom.Name ?? ""}
{subgroupInfo}
";
                }

                await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id,
                    oneDayMessage);
            }

            await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id, "жж", replyMarkup: GetCommonActionsKeyboard());
        }

        private async Task PrintOneDayLessons(DateTime time, RozkladUser user, Message userMessage)
        {
            await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id,
                $"Пары на {LessonsUtils.ConvertDayOfWeekToText(time.DayOfWeek)}");
            await _bot.Client.SendChatActionAsync(userMessage.Chat.Id, ChatAction.Typing);
            var lessons = await _parser.GetLessonsOnDate(user.FacultyShortName, user.ExternalGroupId, time);
            if (!lessons.Any())
            {
                await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id, "Пар нет :)", replyMarkup: GetCommonActionsKeyboard());
            }
            foreach (var lesson in lessons)
            {
                var subgroupInfo = lesson.SubGroup == SubGroup.First ? "1 группа" :
                    lesson.SubGroup == SubGroup.Second ? "2 группа" : "";
                var message = $@"{LessonsUtils.ConvertLessonNumberToText(lesson.LessonNumber)}
{lesson.Subject.Name}
{lesson.Lecturer.FullName ?? ""}
{lesson.Classroom.Name ?? ""}
{subgroupInfo}";
                await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id,
                    message);
            }
            await _bot.Client.SendTextMessageAsync(userMessage.Chat.Id, "пум-пум", replyMarkup: GetCommonActionsKeyboard());
        }

        private ReplyKeyboardMarkup GetWeekScheduleActionsKeyboard() => new[]
        {
            new[] {"На текущую неделю"},
            new[] {"На следующую неделю"},
            new[] {"Назад к меню"},

        };

        private ReplyKeyboardMarkup GetConfirmsButtonsKeyboard() => new[]
        {
            new[] {"Да"},
            new[] {"Нет"}
        };

        private ReplyKeyboardMarkup GetSettingsActionsKeyboard() => new[]
        {
            new[] {"Сбросить настройки"},
            new[] {"Назад к меню"},
        };

        private ReplyKeyboardMarkup GetScheduleActionsKeyboard() => new[]
        {
            new[] {"Расписание на сегодня"},
            new[] {"Расписание на завтра"},
            new[] {"Расписание на неделю"},
            new[] {"Назад к меню"},
        };

        private ReplyKeyboardMarkup GetCommonActionsKeyboard()
        {
            return new[]
            {
                new []{ "Расписание" },
                new []{ "Настройки" }
            };
        }

        private async Task<ReplyKeyboardMarkup> GetGroupsKeyboard(string facultyShortName)
        {
            var groups = await _parser.GetGroups(facultyShortName);

            var groupsRow = new
                List<List<KeyboardButton>>();
            foreach (var group in groups)
            {
                var row = new List<KeyboardButton>();
                row.Add(group.ShortName);
                groupsRow.Add(row);
            }

            ReplyKeyboardMarkup groupsKeyboard = new ReplyKeyboardMarkup(groupsRow);
            return groupsKeyboard;
        }

        private List<List<KeyboardButton>> GetFacultiesKeyBoardRows()
        {
            var faculties = _parser.GetFaculties();
            var rows = new List<List<KeyboardButton>>();
            foreach (var faculty in faculties)
            {
                var row = new List<KeyboardButton>();
                row.Add(faculty.FullName);
                rows.Add(row);
            }

            return rows;
        }
    }
}
