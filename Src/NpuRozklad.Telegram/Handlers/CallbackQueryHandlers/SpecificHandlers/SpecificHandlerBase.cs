using System.Threading.Tasks;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    /// <summary>
    /// <see cref="ISpecificCallbackQueryHandler"/> wrapper that call AnswerCallbackQueryAsync after handling callback
    /// </summary>
    public abstract class SpecificHandlerBase : ISpecificCallbackQueryHandler
    {
        protected readonly ITelegramBotService TelegramBotService;

        protected SpecificHandlerBase(ITelegramBotService telegramBotService)
        {
            TelegramBotService = telegramBotService;
        }
        public async Task Handle(CallbackQueryData callbackQueryData)
        {
            await HandleImplementation(callbackQueryData).ConfigureAwait(false);
            await TelegramBotService.Client.AnswerCallbackQueryAsync(callbackQueryData.CallbackQueryId)
                .ConfigureAwait(false);
        }

        protected abstract Task HandleImplementation(CallbackQueryData callbackQueryData);
    }
}