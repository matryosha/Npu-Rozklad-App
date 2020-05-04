using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class RemoveGroupCallbackHandler : SpecificHandlerBase
    {
        private readonly ITelegramBotActions _botActions;
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ICurrentTelegramUserContext _currentTelegramUserContext;
        private CallbackQueryData _callbackQueryData;
        private string _facultyGroupTypeId;
        private string _facultyTypeId;

        public RemoveGroupCallbackHandler(ITelegramBotActions botActions,
            ITelegramRozkladUserDao telegramRozkladUserDao,
            ICurrentTelegramUserContext currentTelegramUserContext,
            ITelegramBotService telegramBotService) : base(telegramBotService)
        {
            _botActions = botActions;
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _currentTelegramUserContext = currentTelegramUserContext;
        }
        
        protected override async Task HandleImplementation(CallbackQueryData callbackQueryData)
        {
            _callbackQueryData = callbackQueryData;
            ProcessCallbackQueryData();

            var currentUser = _currentTelegramUserContext.TelegramRozkladUser;
            var facultyGroupToRemove =
                currentUser.FacultyGroups
                    .FirstOrDefault(g => 
                        g.TypeId == _facultyGroupTypeId
                        && g.Faculty.TypeId == _facultyTypeId);

            if (facultyGroupToRemove != null)
            {
                currentUser.FacultyGroups.Remove(facultyGroupToRemove);
                await _telegramRozkladUserDao.Update(currentUser);
            }

            await _botActions.ShowTimetableFacultyGroupsRemoveMenu();
        }

        private void ProcessCallbackQueryData()
        {
            var values = _callbackQueryData.Values;
            
            _facultyGroupTypeId = values[0];
            _facultyTypeId = values[1];
        }
    }
}