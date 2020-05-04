using System.Threading.Tasks;
using NpuRozklad.Telegram.Display.Common.Controls;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ResetCurrentUserAction
    {
        private readonly MainMenuCreator _mainMenuCreator;
        private readonly ICurrentTelegramUserContext _currentTelegramUserContext;
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ITelegramBotService _telegramBotService;

        public ResetCurrentUserAction(MainMenuCreator mainMenuCreator,
            ICurrentTelegramUserContext currentTelegramUserContext,
            ITelegramRozkladUserDao telegramRozkladUserDao,
            ITelegramBotService telegramBotService)
        {
            _mainMenuCreator = mainMenuCreator;
            _currentTelegramUserContext = currentTelegramUserContext;
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _telegramBotService = telegramBotService;
        }

        public async Task Execute(ResetCurrentUserOptions options = null)
        {
            var replyKeyboard = _mainMenuCreator.CreateMenu();
            const string messageText = "Reset";

            await _telegramRozkladUserDao.Delete(_currentTelegramUserContext.TelegramRozkladUser);

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