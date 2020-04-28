using System.Threading.Tasks;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.Handlers.MessageHandlers
{
    public class MessageTextHandler
    {
        private readonly ITelegramBotActions _telegramBotActions;
        private readonly ILocalizationService _localizationService;
        private readonly ICurrentTelegramUserService _currentUserService;

        private string UserLang => _currentUserService.Language;
        
        public MessageTextHandler(ITelegramBotActions telegramBotActions,
            ILocalizationService localizationService,
            ICurrentTelegramUserService currentUserService)
        {
            _telegramBotActions = telegramBotActions;
            _localizationService = localizationService;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(Message message)
        {
            bool isHandled = false;

            var messageText = message.Text;
            
            if (messageText == _localizationService[UserLang, "schedule-reply-keyboard"])
            {
                await _telegramBotActions.ShowTimetableFacultyGroupsMenu();
                isHandled = true;
            }

            return isHandled;
        }
    }
}