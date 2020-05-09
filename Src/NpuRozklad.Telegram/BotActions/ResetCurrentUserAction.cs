using System.Threading.Tasks;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ResetCurrentUserAction
    {
        private readonly MainMenuCreator _mainMenuCreator;
        private readonly ICurrentTelegramUserProvider _currentTelegramUserProvider;
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ITelegramBotService _telegramBotService;

        public ResetCurrentUserAction(MainMenuCreator mainMenuCreator,
            ICurrentTelegramUserProvider currentTelegramUserProvider,
            ITelegramRozkladUserDao telegramRozkladUserDao,
            ITelegramBotService telegramBotService)
        {
            _mainMenuCreator = mainMenuCreator;
            _currentTelegramUserProvider = currentTelegramUserProvider;
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _telegramBotService = telegramBotService;
        }

        public async Task Execute(ResetCurrentUserOptions options = null)
        {
            var replyKeyboard = _mainMenuCreator.CreateMenu();
            const string messageText = "Reset";

            var currentUser = _currentTelegramUserProvider.GetCurrentTelegramRozkladUser();
            await _telegramRozkladUserDao.Delete(currentUser);

            await _telegramBotService.SendOrEditMessageAsync(
                messageText,
                replyMarkup: replyKeyboard,
                forceNewMessage: true);
        }
    }

    public class ResetCurrentUserOptions
    {
    }
}