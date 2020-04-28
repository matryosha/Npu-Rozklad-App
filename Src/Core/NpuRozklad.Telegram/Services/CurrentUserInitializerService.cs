using System;
using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NpuRozklad.Telegram.Services
{
    public class CurrentUserInitializerService : ICurrentUserInitializerService
    {
        private readonly ICurrentTelegramUserService _currentTelegramUserService;
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ILocalizationService _localizationService;

        public CurrentUserInitializerService(ICurrentTelegramUserService currentTelegramUserService,
            ITelegramRozkladUserDao telegramRozkladUserDao,
            ILocalizationService localizationService)
        {
            _currentTelegramUserService = currentTelegramUserService;
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _localizationService = localizationService;
        }
        
        public async Task InitializeCurrentUser(Update update)
        {
            int userTelegramId;
            long chatId;
            switch (update.Type)
            {
                case UpdateType.Message:
                    userTelegramId = update.Message.From.Id;
                    chatId = update.Message.Chat.Id;
                    break;
                case UpdateType.CallbackQuery:
                    userTelegramId = update.CallbackQuery.From.Id;
                    chatId = update.CallbackQuery.Message.Chat.Id;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var telegramRozkladUser = await _telegramRozkladUserDao.FindByTelegramId(userTelegramId);

            if (telegramRozkladUser == null)
            {
                telegramRozkladUser = new TelegramRozkladUser
                {
                    TelegramId = userTelegramId,
                    Language = _localizationService.DefaultLanguage
                };

                await _telegramRozkladUserDao.Add(telegramRozkladUser);
            }

            if (telegramRozkladUser.IsDeleted)
            {
                telegramRozkladUser.IsDeleted = false;

                await _telegramRozkladUserDao.Update(telegramRozkladUser);
            }

            _currentTelegramUserService.TelegramRozkladUser = telegramRozkladUser;
            _currentTelegramUserService.ChatId = chatId;
        }
    }
}