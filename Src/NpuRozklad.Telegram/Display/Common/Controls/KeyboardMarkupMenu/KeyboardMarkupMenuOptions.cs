using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls.KeyboardMarkupMenu
{
    public class KeyboardMarkupMenuOptions
    {
        public ICollection<ICollection<string>> Items { get; set; }
        public ICollection<KeyboardButton> AdditionalButtons { get; set; } = new List<KeyboardButton>();
        public bool OneTimeKeyboard { get; set; }
    }
}