using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RozkladNpuAspNetCore.Configurations;
using RozkladNpuAspNetCore.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Services
{
    public class BotService : IBotService
    {
        public BotService(IOptions<BotConfiguration> botOptions)
        {
            var config = botOptions.Value;
            Client = new TelegramBotClient(config.BotApi);
        }
        public ITelegramBotClient Client { get; private set; }

        public async Task SendErrorMessage(ChatId chatId)
        {
            await Client.SendTextMessageAsync(chatId, "Something went wrong :(");
        }

    }
}
