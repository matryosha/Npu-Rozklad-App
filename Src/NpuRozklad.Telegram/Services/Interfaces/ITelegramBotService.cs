using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Services.Interfaces
{
    public interface ITelegramBotService
    {
        ITelegramBotClient Client { get; }

        Task<Message> SendOrEditMessageAsync(
            ChatId chatId,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default,
            bool forceNewMessage = false
        );
    }
}