using System;
using System.Threading.Tasks;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.BotActions
{
    public class ShowMessageAction
    {
        private readonly ITelegramBotService _telegramBotService;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public ShowMessageAction(ITelegramBotService telegramBotService,
            ICurrentUserLocalizationService currentUserLocalizationService
            )
        {
            _telegramBotService = telegramBotService;
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        
        public Task Execute(Action<ShowMessageOptions> optionsBuilder)
        {
            var options = new ShowMessageOptions();
            optionsBuilder(options);
            
            var messageText = GetMessageText(options);

            return _telegramBotService.SendOrEditMessageAsync(
                messageText,
                forceNewMessage: true);        
        }

        private string GetMessageText(ShowMessageOptions options)
        {
            if (options.ShowIncorrectInputMessage)
                return _currentUserLocalizationService["incorrect-input"];

            return _currentUserLocalizationService[options.MessageTextLocalizationValue];
        }
    }

    public class ShowMessageOptions
    {
        public string MessageTextLocalizationValue { get; set; }
        public bool ShowIncorrectInputMessage { get; set; }
    }
}