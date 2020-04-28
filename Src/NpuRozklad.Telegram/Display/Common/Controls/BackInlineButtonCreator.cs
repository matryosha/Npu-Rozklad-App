using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Helpers;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Display.Common.Controls
{
    public class BackInlineButtonCreator
    {
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentUserService;

        public BackInlineButtonCreator(ILocalizationService localizationService,
            ICurrentTelegramUserService currentUserService)
        {
            _localizationService = localizationService;
            _currentUserService = currentUserService;
        }

        public InlineKeyboardButton Create(CallbackQueryActionType backActionCallBack, string callbackData = null)
        {
            var callbackDataText = string.IsNullOrWhiteSpace(callbackData)
                ? CallbackDataFormatter.ToCallBackData(backActionCallBack)
                : $"{CallbackDataFormatter.ToCallBackData(backActionCallBack)};{callbackData}";
            
            return new InlineKeyboardButton
            {
                Text = _localizationService[_currentUserService.Language, "back"],
                CallbackData = callbackDataText
            };
        }
    }
}