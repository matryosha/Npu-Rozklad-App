using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.Telegram.BotActions;
using NpuRozklad.Telegram.Persistence;
using Telegram.Bot.Types;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class TimetableSelectingFacultyGroupToAddActionHandler : ILongLastingUserActionHandler
    {
        private readonly ITelegramRozkladUserDao _telegramRozkladUserDao;
        private readonly ITelegramBotActions _telegramBotActions;
        private readonly IFacultyGroupsProvider _facultyGroupsProvider;
        private readonly ILongLastingUserActionManager _longLastingUserActionManager;

        public TimetableSelectingFacultyGroupToAddActionHandler(ITelegramRozkladUserDao telegramRozkladUserDao,
            ITelegramBotActions telegramBotActions, IFacultyGroupsProvider facultyGroupsProvider,
            ILongLastingUserActionManager longLastingUserActionManager)
        {
            _telegramRozkladUserDao = telegramRozkladUserDao;
            _telegramBotActions = telegramBotActions;
            _facultyGroupsProvider = facultyGroupsProvider;
            _longLastingUserActionManager = longLastingUserActionManager;
        }
        public async Task<bool> Handle(LongLastingUserActionArguments userActionArguments)
        {
            var selectedFacultyGroupTypeId = (userActionArguments.Parameters[typeof(Message)] as Message)?.Text;
            
            if (string.IsNullOrWhiteSpace(selectedFacultyGroupTypeId))
            {
                await _telegramBotActions.ShowIncorrectInputMessage();
                return true;
            }
            
            // check for back/main button
            
            var facultyGroups = await GetFacultyGroups(userActionArguments);
            var facultyGroup = facultyGroups.FirstOrDefault(f => f.Name == selectedFacultyGroupTypeId);

            if (facultyGroup == null)
            {
                await _telegramBotActions.ShowIncorrectInputMessage();
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