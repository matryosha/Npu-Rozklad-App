using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RozkladNpuAspNetCore.Utils;
using Telegram.Bot;

namespace RozkladNpuAspNetCore.Services
{
    public class BotService
    {
        private readonly BotConfiguration _config;
        public BotService(IOptions<BotConfiguration> botOptions)
        {
            _config = botOptions.Value;
            Client = new TelegramBotClient(_config.BotApi);
        }
        public TelegramBotClient Client { get; private set; }

    }
}
