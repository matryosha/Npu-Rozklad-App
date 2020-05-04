using System;
using System.Threading;
using System.Threading.Tasks;
using NpuRozklad.Telegram.Interfaces;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NpuRozklad.Telegram.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly ICurrentScopeServiceProvider _currentScopeServiceProvider;

        public TelegramBotService(string botApiToken,
            ICurrentScopeServiceProvider currentScopeServiceProvider)
        {
            _currentScopeServiceProvider = currentScopeServiceProvider;
            Client = new TelegramBotClient(botApiToken);
        }

        public ITelegramBotClient Client { get; }

        public async Task<Message> SendOrEditMessageAsync(ChatId chatId, string text, ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false, bool disableNotification = false, int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null, CancellationToken cancellationToken = default, bool forceNewMessage = false)
        {
            if (forceNewMessage)
                return await SendMessage();

            var currentScopeMessageIdProvider = _currentScopeServiceProvider.GetService<ICurrentScopeMessageIdProvider>();
            var currentMessageId = currentScopeMessageIdProvider.MessageId;
            if (currentMessageId == null) return await SendMessage();

            if (replyMarkup is InlineKeyboardMarkup keyboardMarkup)
                try
                {
                    return await Client.EditMessageTextAsync(
                        chatId, (int) currentMessageId, text, parseMode, disableWebPagePreview, keyboardMarkup,
                        cancellationToken);
                }
                catch (MessageIsNotModifiedException)
                {
                    // dos
                    return null;
                }

            await Client.DeleteMessageAsync(chatId, (int) currentMessageId, cancellationToken);
            return await SendMessage();

            Task<Message> SendMessage()
            {
                return Client.SendTextMessageAsync(chatId, text, parseMode, disableWebPagePreview, disableNotification,
                    replyToMessageId, replyMarkup, cancellationToken);
            }
        }
    }
}