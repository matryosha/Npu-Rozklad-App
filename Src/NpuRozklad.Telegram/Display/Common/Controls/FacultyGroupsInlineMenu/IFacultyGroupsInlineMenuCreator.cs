using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls.FacultyGroupsInlineMenu
{
    public interface IFacultyGroupsInlineMenuCreator
    {
        InlineKeyboardMarkup CreateMenu(FacultyGroupsInlineMenuOptions options);
    }
}