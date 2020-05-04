using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowApplicationVersionAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly IApplicationVersionProvider _applicationVersionProvider;
        private readonly MainMenuCreator _mainMenuCreator;
        private readonly ICurrentTelegramUserService _currentTelegramUserService;

        public ShowApplicationVersionAction(ITelegramBotService telegramBotService,
            IApplicationVersionProvider applicationVersionProvider,
            MainMenuCreator mainMenuCreator,
            ICurrentTelegramUserService currentTelegramUserService)
        {
            _telegramBotService = telegramBotService;
            _applicationVersionProvider = applicationVersionProvider;
            _mainMenuCreator = mainMenuCreator;
            _currentTelegramUserService = currentTelegramUserService;
        }
        public Task Execute()
        {
            var replyKeyboard = _mainMenuCreator.CreateMenu();
            var versionText = _applicationVersionProvider.GetApplicationVersion();

            return _telegramBotService.SendOrEditMessageAsync(
                versionText,
                replyMarkup: replyKeyboard,
                forceNewMessage: true);
        }
    }
}