using System;
using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Exceptions;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NpuRozklad.Telegram.Services
{
    public class CurrentUserInitializerService : ICurrentUserInitializerService
    {
        private readonly ICurrentTelegramUserContext _currentTelegramUserContext;
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ILocalizationService _localizationService;

        public CurrentUserInitializerService(ICurrentTelegramUserContext currentTelegramUserContext,
            ITelegramRozkladUserDao telegramRozkladUserDao,
            ILocalizationService localizationService)
        {
            _currentTelegramUserContext = currentTelegramUserContext;
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
                case UpdateType.EditedMessage:
                    userTelegramId = update.EditedMessage.From.Id;
                    chatId = update.EditedMessage.Chat.Id;
                    break;
                default:
                    throw new CurrentUserInitializationException($"Unknown telegram update type: {update.Type}");
            }

            try
            {
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

                _currentTelegramUserContext.TelegramRozkladUser = telegramRozkladUser;
                _currentTelegramUserContext.ChatId = chatId;
            }
            catch (Exception e)
            {
                throw new CurrentUserInitializationException("Error when getting user", e);
            }
        }
    }
}