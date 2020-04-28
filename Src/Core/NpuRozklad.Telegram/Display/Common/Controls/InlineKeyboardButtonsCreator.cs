using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls
{
    public class InlineKeyboardButtonsCreator
    {
        public InlineKeyboardButton[] Create(InlineKeyboardButtonsCreatorOptions options)
        {
            var numberOfButtons = options.NumberOfButtons;
            var callbackDataFunc = options.CallbackDataFunc;
            var buttonTextFunc = options.ButtonTextFunc;
            var result = new InlineKeyboardButton[numberOfButtons];

            for (int i = 0; i < numberOfButtons; i++)
                result[i] = new InlineKeyboardButton {Text = buttonTextFunc(i), CallbackData = callbackDataFunc(i)};

            return result;
        }
    }

    public class InlineKeyboardButtonsCreatorOptions
    {
        public int NumberOfButtons { get; set; }

        /// <summary>
        /// Takes iteration returns callback data string
        /// </summary>
        public Func<int, string> CallbackDataFunc { get; set; }

        public Func<int, string> ButtonTextFunc { get; set; }
    }
}