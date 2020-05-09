using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls
{
    public class InlineKeyboardButtonsCreator
    {
        public InlineKeyboardButton[][] Create(Action<InlineKeyboardButtonsCreatorOptions> optionsBuilder)
        {
            var options = new InlineKeyboardButtonsCreatorOptions();
            optionsBuilder(options);

            var itemsNumber = options.ItemsNumber;
            var callbackDataFunc = options.CallbackDataFunc;
            var buttonTextFunc = options.ButtonTextFunc;
            var maxButtonsInRow = options.MaxButtonsInRow;
            var additionalButtons = options.AdditionalButtons;

            var isShouldAppendAdditionalButtons = additionalButtons?.Any() ?? false;

            var rowsCount = (int) Math.Ceiling((double) itemsNumber / maxButtonsInRow);

            var result = new InlineKeyboardButton[isShouldAppendAdditionalButtons ? rowsCount + 1 : rowsCount][];
            var loopsIteration = 0;

            for (var rowNumber = 0; rowNumber < rowsCount; rowNumber++)
            {
                // ˢʳʸ ᶠᵒʳ ᵗʰᵃᵗ •ㅅ•
                int numberOfItemsInCurrentRow;

                if (maxButtonsInRow > itemsNumber)
                {
                    numberOfItemsInCurrentRow = itemsNumber;
                }
                else
                {
                    var numberOfProcessedItems = loopsIteration;
                    var subtractionDifference = itemsNumber - numberOfProcessedItems;

                    numberOfItemsInCurrentRow = subtractionDifference < maxButtonsInRow
                        ? subtractionDifference
                        : maxButtonsInRow;
                }

                result[rowNumber] = new InlineKeyboardButton[numberOfItemsInCurrentRow];

                for (var rowItemNumber = 0; rowItemNumber < numberOfItemsInCurrentRow; rowItemNumber++)
                {
                    result[rowNumber][rowItemNumber] = new InlineKeyboardButton
                    {
                        Text = buttonTextFunc(loopsIteration),
                        CallbackData = callbackDataFunc(loopsIteration)
                    };
                    loopsIteration++;
                }
            }

            if (isShouldAppendAdditionalButtons)
                result[result.Length - 1] = additionalButtons.ToArray();

            return result;
        }
    }

    public class InlineKeyboardButtonsCreatorOptions
    {
        public int ItemsNumber { get; set; }
        public int MaxButtonsInRow { get; set; } = 10;

        /// <summary>
        /// Takes iteration returns callback data string
        /// </summary>
        public Func<int, string> CallbackDataFunc { get; set; }

        public Func<int, string> ButtonTextFunc { get; set; }
        public ICollection<InlineKeyboardButton> AdditionalButtons { get; set; }
    }
}