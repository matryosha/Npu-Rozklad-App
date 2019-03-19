using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RozkladNpuAspNetCore.Configurations;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RozkladNpuAspNetCore.Services
{
    public class BotService
    {
        public BotService(IOptions<BotConfiguration> botOptions)
        {
            var config = botOptions.Value;
            Client = new TelegramBotClient(config.BotApi);
        }
        public TelegramBotClient Client { get; private set; }

        public async Task SendErrorMessage(ChatId chatId)
        {
            await Client.SendTextMessageAsync(chatId, "Something went wrong :(");
        }

    }
}
