using System.Threading.Tasks;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.MessageHandlers
{
    public class MessageTextHandler
    {
        private readonly ITelegramBotActions _telegramBotActions;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public MessageTextHandler(ITelegramBotActions telegramBotActions,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _telegramBotActions = telegramBotActions;
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        public async Task<bool> Handle(Message message)
        {
            var messageText = message.Text;
            
            if (messageText == _currentUserLocalizationService["schedule-reply-keyboard"])
            {
                await _telegramBotActions.ShowTimetableFacultyGroupsMenu();
                return true;
            }

            await _telegramBotActions.ShowMainMenu();
            return true;
        }
    }
}