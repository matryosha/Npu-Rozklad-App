using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services.Interfaces;

namespace NpuRozklad.Telegram.Handlers.CallbackQueryHandlers.SpecificHandlers
{
    public class RemoveGroupCallbackHandler : ISpecificCallbackQueryHandler
    {
        private readonly ITelegramBotActions _botActions;
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ICurrentTelegramUserService _currentTelegramUserService;
        private CallbackQueryData _callbackQueryData;
        private string _facultyGroupTypeId;
        private string _facultyTypeId;

        public RemoveGroupCallbackHandler(ITelegramBotActions botActions,
            ITelegramRozkladUserDao telegramRozkladUserDao,
            ICurrentTelegramUserService currentTelegramUserService)
        {
            _botActions = botActions;
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _currentTelegramUserService = currentTelegramUserService;
        }
        
        public async Task Handle(CallbackQueryData callbackQueryData)
        {
            _callbackQueryData = callbackQueryData;
            ProcessCallbackQueryData();

            var currentUser = _currentTelegramUserService.TelegramRozkladUser;
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