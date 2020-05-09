using NpuRozklad.Telegram.Helpers;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls
{
    public class BackInlineButtonCreator
    {
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public BackInlineButtonCreator(ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _currentUserLocalizationService = currentUserLocalizationService;
        }

        public InlineKeyboardButton Create(CallbackQueryActionType backActionCallBack, string callbackData = null)
        {
            var callbackDataText = string.IsNullOrWhiteSpace(callbackData)
                ? CallbackDataFormatter.ToCallBackData(backActionCallBack)
                : $"{CallbackDataFormatter.ToCallBackData(backActionCallBack)};{callbackData}";
            
            return new InlineKeyboardButton
            {
                Text = _currentUserLocalizationService["back"],
                CallbackData = callbackDataText
            };
        }
    }
}