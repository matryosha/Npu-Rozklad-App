using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuBot.Utils;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace RozkladNpuBot
{
    class Start
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("");
        private static readonly NpuParser Parser = new NpuParser();
        private static readonly List<User> NewUsers = new List<User>();


        public static void Main(string[] args)
        {

            Bot.OnMessage += MessageReceived;
            Bot.OnCallbackQuery += CallBackReceived;
            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.ReadLine();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        private static void CallBackReceived(object sender, CallbackQueryEventArgs e)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new [] // first row
                {
                    InlineKeyboardButton.WithCallbackData("Cегодня"),
                    InlineKeyboardButton.WithCallbackData("Завтра"),
                }
            });

            switch (e.CallbackQuery.Data)
            {
                case "Cегодня":
                    var faculty = e.CallbackQuery.Message.Text.Split(';')[1];
                    var groupName = e.CallbackQuery.Message.Text.Split(';')[0];
                    var lessons = Parser.GetLessonsOnDate(faculty, groupName, DateTime.Today).Result;

                    Bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                        "Пары на сегодня:");
                    foreach (var lesson in lessons)
                    {
                        var message = $@"{LessonsUtils.ConvertLessonNumberToText(lesson.LessonNumber)}
{lesson.Subject.Name}
{lesson.Lecturer.FullName}
{lesson.Classroom.Name}
{lesson.SubGroup.ToString()}";
                        Bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            message);
                    }
                    Bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"{groupName};{faculty}", replyMarkup: inlineKeyboard);
                    return;
                    break;
                case "Завтра":
                    var faculty2 = e.CallbackQuery.Message.Text.Split(';')[1];
                    var groupName2 = e.CallbackQuery.Message.Text.Split(';')[0];
                    var lessons2 = Parser.GetLessonsOnDate(faculty2, groupName2, DateTime.Today.AddDays(1)).Result;
                    Bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                        "Пары на завтра:");
                    foreach (var lesson in lessons2)
                    {
                        var message = $@"{LessonsUtils.ConvertLessonNumberToText(lesson.LessonNumber)}
{lesson.Subject.Name}
{lesson.Lecturer.FullName}
{lesson.Classroom.Name}
{lesson.SubGroup.ToString()}";
                        Bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                            message);
                    }
                    Bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"{groupName2};{faculty2}", replyMarkup: inlineKeyboard);
                    return;
                    break;
            }
            
            Bot.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"{e.CallbackQuery.Data};{e.CallbackQuery.Message.Text.Split('.')[0]}", replyMarkup: inlineKeyboard);
        }

        static async void MessageReceived(object sender, MessageEventArgs args)
        {
            var message = args.Message;

            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            var newUser = NewUsers.FirstOrDefault(u => u.Id == message.Chat.Id);
            if (newUser != null)
            {
                if (string.IsNullOrEmpty(newUser.FacultyShortName))
                {
                    var faculty = Parser.GetFaculties().FirstOrDefault(f => f.FullName == message.Text);
                    if (faculty != null)
                    {
                        var groupShortName = Parser.GetFaculties()
                            .FirstOrDefault(f => f.FullName == message.Text)
                            .ShortName;

                        newUser.FacultyShortName = faculty.ShortName;
                        var groups = await Parser.GetGroups(groupShortName);

                        var rows = new List<List<InlineKeyboardButton>>();
                        foreach (var group in groups)
                        {
                            var row = new List<InlineKeyboardButton>();
                            row.Add(InlineKeyboardButton.WithCallbackData(group.ShortName));
                            rows.Add(row);
                        }

                        InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(rows);

                        await Bot.SendTextMessageAsync(message.Chat.Id, $"{faculty.ShortName}. Теперь группа:", replyMarkup: keyboard);
                        return;
                    }
                }

                if (string.IsNullOrEmpty(newUser.Group))
                {
                    //var group = Parser.G
                }

            }

            switch (message.Text.Split(' ').First())
            {
                case "/start":
                    var user = new User
                    {
                        Id = message.Chat.Id
                    };
                    NewUsers.Add(user);
                    var answer = @"Йо. Выбери факультет";
                    var faculties = Parser.GetFaculties();
                    var rows = new List<List<KeyboardButton>>();
                    foreach (var faculty in faculties)
                    {
                        var row = new List<KeyboardButton>();
                        row.Add(faculty.FullName);
                        rows.Add(row);
                    }
                    ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(rows);

                    await Bot.SendTextMessageAsync(message.Chat.Id, answer, replyMarkup: keyboard);
                    break;
                case "фак выбран, выбери группу:":

                    break;
            }

        }

        }

    class User
    {
        public long Id { get; set; }
        public string FacultyShortName { get; set; }
        public string Group { get; set; }
    }
    }
