using System.Collections.Generic;
using NpuTimetableParser;
using Telegram.Bot.Types.ReplyMarkups;

namespace RozkladNpuAspNetCore.Infrastructure
{
    public class ReplyKeyboardMarkupCreator
    {
        public ReplyKeyboardMarkup FacultiesMarkup(List<Faculty> faculties)
        {
            var rows = new List<List<KeyboardButton>>();
            foreach (var faculty in faculties)
            {
                var row = new List<KeyboardButton>();
                row.Add(faculty.FullName);
                rows.Add(row);
            }

            rows.Add(new List<KeyboardButton>
            {
                "Menu"
            });

            return new ReplyKeyboardMarkup(rows);
        }

        public ReplyKeyboardMarkup GroupsMarkup(List<Group> groups)
        {
            var groupsRow = new
                List<List<KeyboardButton>>();
            foreach (var group in groups)
            {
                var row = new List<KeyboardButton> { group.ShortName };
                groupsRow.Add(row);
            }

            groupsRow.Add(new List<KeyboardButton>
            {
                "Menu"
            });

            return new ReplyKeyboardMarkup(groupsRow);
        }

        public ReplyKeyboardMarkup MainMenuMarkup()
        {
            return new[]
            {
                new []{ "Schedule" }
            };
        }
    }
}
