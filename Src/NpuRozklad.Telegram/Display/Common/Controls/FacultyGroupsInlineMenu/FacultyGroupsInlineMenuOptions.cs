using System.Collections.Generic;
using NpuRozklad.Core.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls.FacultyGroupsInlineMenu
{
    public class FacultyGroupsInlineMenuOptions
    {
        public ICollection<Group> FacultyGroups { get; set; }
        public CallbackQueryActionType CallbackActionType { get; set; }
        public InlineKeyboardButton[] AdditionalButtons { get; set; }
    }
}