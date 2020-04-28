using System.Threading.Tasks;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ResetCurrentUserAction
    {
        private readonly MainMenuCreator _mainMenuCreator;
        private readonly ICurrentTelegramUserService _currentTelegramUserService;
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ITelegramBotService _telegramBotService;

        public ResetCurrentUserAction(MainMenuCreator mainMenuCreator,
            ICurrentTelegramUserService currentTelegramUserService,
            ITelegramRozkladUserDao telegramRozkladUserDao,
            ITelegramBotService telegramBotService)
        {
            _mainMenuCreator = mainMenuCreator;
            _currentTelegramUserService = currentTelegramUserService;
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _telegramBotService = telegramBotService;
        }

        public async Task Execute(ResetCurrentUserOptions options = null)
        {
            var replyKeyboard = _mainMenuCreator.CreateMenu();
            const string messageText = "Reset";

            await _telegramRozkladUserDao.Delete(_currentTelegramUserService.TelegramRozkladUser);

            await _telegramBotService.Client.SendTextMessageAsync(
                _currentTelegramUserService.ChatId,
                messageText,
                replyMarkup: replyKeyboard);
        }
    }

    public class ResetCurrentUserOptions
    {
    }
}