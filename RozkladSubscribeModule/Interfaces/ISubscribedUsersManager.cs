using System.Collections.Generic;
using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISubscribedUsersManager
    {
        Task AddUser(int telegramId, int groupExternalId);

        Task DeleteUser(int telegramId, int groupExternalId);

        List<SubscribedUser> GetUsers();
    }
}