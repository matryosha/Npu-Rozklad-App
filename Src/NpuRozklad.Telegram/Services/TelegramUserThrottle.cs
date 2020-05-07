using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Services
{
    public class TelegramUserThrottle : ITelegramUserThrottle
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly IMemoryCache _memoryCache;


        public TelegramUserThrottle(ITelegramBotService telegramBotService)
        {
            _telegramBotService = telegramBotService;
            _memoryCache = new MemoryCache(new MemoryCacheOptions
            {
                Clock = new SystemClock(),
                ExpirationScanFrequency = TimeSpan.FromMilliseconds(200)
            });
        }
        
        public bool ShouldSkipProcessing(Update update)
        {
            var userTelegramId = update.CallbackQuery?.From.Id
                                 ?? update.Message?.From.Id
                                 ?? update.EditedMessage?.From.Id;

            if (userTelegramId == null) return true;
            if (_memoryCache.TryGetValue(userTelegramId, out _))
            {
                var callbackId = update.CallbackQuery?.Id;
                if (callbackId != null)
                    Task.Run(()=> _telegramBotService.Client.AnswerCallbackQueryAsync(callbackId, "NO DoS (・`ω´・)"));
                return true;
            }

            _memoryCache.Set(userTelegramId, new object(), TimeSpan.FromMilliseconds(500));
            return false;
        }
    }
}