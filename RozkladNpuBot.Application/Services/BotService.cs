using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RozkladNpuBot.Application.Configurations;
using RozkladNpuBot.Application.Interfaces;
using RozkladNpuBot.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RozkladNpuBot.Application.Services
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
