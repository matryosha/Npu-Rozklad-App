using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CurrentUserInitializerService> _logger;

        public CurrentUserInitializerService(ICurrentTelegramUserContext currentTelegramUserContext,
            ITelegramRozkladUserDao telegramRozkladUserDao,
            ILocalizationService localizationService,
            ILogger<CurrentUserInitializerService> logger)
        {
            _currentTelegramUserContext = currentTelegramUserContext;
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _localizationService = localizationService;
            _logger = logger;
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

            TelegramRozkladUser telegramRozkladUser = null;
            try
            {
                telegramRozkladUser = await _telegramRozkladUserDao.FindByTelegramId(userTelegramId);

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
                var outE =  new CurrentUserInitializationException("Error when getting user", e);

                var telegramRozkladUserId = telegramRozkladUser?.TelegramId;
                _logger.LogError(TelegramLogEvents.UserInitializationError, outE,
                    "userTelegramId = {userTelegramId}. " +
                    "chatId = {chatId}. " + 
                    "TelegramRozkladUser telegram id = {telegramRozkladUserId}", 
                    userTelegramId, chatId, telegramRozkladUserId);
                
                throw outE;
            }
        }
    }
}