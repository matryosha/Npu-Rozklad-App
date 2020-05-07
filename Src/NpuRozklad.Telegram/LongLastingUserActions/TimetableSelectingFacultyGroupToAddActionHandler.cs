using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.BotActions;
using NpuRozklad.Telegram.Persistence;
using NpuRozklad.Telegram.Services.Interfaces;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class TimetableSelectingFacultyGroupToAddActionHandler : ILongLastingUserActionHandler
    {
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ITelegramBotActions _telegramBotActions;
        private readonly IFacultyGroupsProvider _facultyGroupsProvider;
        private readonly ILongLastingUserActionManager _longLastingUserActionManager;
        private readonly ICurrentUserLocalizationService _currentUserLocalizationService;

        public TimetableSelectingFacultyGroupToAddActionHandler(ITelegramRozkladUserDao telegramRozkladUserDao,
            ITelegramBotActions telegramBotActions, IFacultyGroupsProvider facultyGroupsProvider,
            ILongLastingUserActionManager longLastingUserActionManager,
            ICurrentUserLocalizationService currentUserLocalizationService)
        {
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _telegramBotActions = telegramBotActions;
            _facultyGroupsProvider = facultyGroupsProvider;
            _longLastingUserActionManager = longLastingUserActionManager;
            _currentUserLocalizationService = currentUserLocalizationService;
        }
        public async Task<bool> Handle(LongLastingUserActionArguments userActionArguments)
        {
            var userInput = (userActionArguments.Parameters[typeof(Message)] as Message)?.Text;
            
            if (string.IsNullOrWhiteSpace(userInput))
            {
                await _telegramBotActions.ShowMessage(o => o.ShowIncorrectInputMessage = true);
                return true;
            }

            if (userInput == _currentUserLocalizationService["back"])
            {
                await _telegramBotActions.ShowTimetableSelectingFacultyMenu();
                return true;
            }
          
            var facultyGroups = await GetFacultyGroups(userActionArguments);
            var facultyGroup = facultyGroups.FirstOrDefault(f => f.Name == userInput);

            if (facultyGroup == null)
            {
                await _telegramBotActions.ShowMessage(o => 
                    o.MessageTextLocalizationValue = "such-faculty-group-was-not-found");
                return true;
            }

            var currentTelegramUser = userActionArguments.TelegramRozkladUser;
            
            currentTelegramUser.FacultyGroups.Add(facultyGroup);
            await _telegramRozkladUserDao.Update(currentTelegramUser);
            await _longLastingUserActionManager.ClearUserAction(currentTelegramUser);

            await _telegramBotActions.ShowMainMenu(new ShowMainMenuOptions
            {
                LocalizationValueToShow = "faculty-group-has-been-added"
            });

            return true;
        }

        private async Task<ICollection<Group>> GetFacultyGroups(LongLastingUserActionArguments userActionArguments)
        {
            if (userActionArguments.Parameters[typeof(ICollection<Group>)] is ICollection<Group> facultyGroups)
                return facultyGroups;

            if (!(userActionArguments.Parameters[typeof(Faculty)] is Faculty faculty))
                throw new ArgumentNullException(nameof(faculty), 
                    $"{nameof(LongLastingUserActionArguments)}.{nameof(userActionArguments.Parameters)}[{nameof(Faculty)}] is null");

            facultyGroups = await _facultyGroupsProvider.GetFacultyGroups(faculty);

            return facultyGroups;
        }
    }
}