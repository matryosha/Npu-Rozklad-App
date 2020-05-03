using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using static NpuRozklad.Telegram.Helpers.CallbackDataFormatter;


namespace NpuRozklad.Telegram.Display.Common.Controls.FacultyGroupsInlineMenu
{
    public class FacultyGroupsInlineMenuCreator : IFacultyGroupsInlineMenuCreator
    {
        private readonly InlineKeyboardButtonsCreator _inlineKeyboardButtonsCreator;
        private int _maxButtonsInRow = 3;
        public FacultyGroupsInlineMenuCreator(InlineKeyboardButtonsCreator inlineKeyboardButtonsCreator)
        {
            _inlineKeyboardButtonsCreator = inlineKeyboardButtonsCreator;
        }
        
        public InlineKeyboardMarkup CreateMenu(FacultyGroupsInlineMenuOptions options)
        {
            var facultyGroups = options.FacultyGroups.ToArray();
            var callbackActionType = options.CallbackActionType;
            var additionalButtons = options.AdditionalButtons;

            var result = _inlineKeyboardButtonsCreator.Create(o =>
            {
                o.MaxButtonsInRow = _maxButtonsInRow;
                o.AdditionalButtons = additionalButtons;
                o.ItemsNumber = facultyGroups.Length;
                o.ButtonTextFunc = i => facultyGroups[i].Name;
                o.CallbackDataFunc = i => ToCallBackData(callbackActionType, facultyGroups[i]);
            });

            return new InlineKeyboardMarkup(result);
        }
    }
}