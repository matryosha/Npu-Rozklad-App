using System.Collections.Generic;
using System.Linq;
using NpuRozklad.Core.Entities;
using Telegram.Bot.Types.ReplyMarkups;
using static NpuRozklad.Telegram.Helpers.CallbackDataFormatter;


namespace NpuRozklad.Telegram.Display.Common.Controls.FacultyGroupsInlineMenu
{
    public class FacultyGroupsInlineMenuCreator : IFacultyGroupsInlineMenuCreator
    {
        private int _maxButtonsInRow = 2;
        public InlineKeyboardMarkup CreateMenu(FacultyGroupsInlineMenuOptions options)
        {
            var groups = options.FacultyGroups;
            var callbackActionType = options.CallbackActionType;
            
            var result = new List<List<InlineKeyboardButton>>();
            var rowButtons = new List<InlineKeyboardButton>(2);

            foreach (var facultyGroup in groups)
            {
                var button = CreateFacultyGroupButton(callbackActionType, facultyGroup);

                if (rowButtons.Count > _maxButtonsInRow)
                {
                    result.Add(rowButtons);
                    rowButtons = new List<InlineKeyboardButton>(_maxButtonsInRow);
                }

                rowButtons.Add(button);
            }
            
            result.Add(rowButtons);
            result.Add(options.AdditionalButtons.ToList());

            return new InlineKeyboardMarkup(result);
        }
        
        private static InlineKeyboardButton CreateFacultyGroupButton(CallbackQueryActionType callbackQueryActionType, Group facultyGroup)
        {
            return new InlineKeyboardButton
            {
                Text = facultyGroup.Name,
                CallbackData = ToCallBackData(callbackQueryActionType, facultyGroup)
            };
        }
    }
}