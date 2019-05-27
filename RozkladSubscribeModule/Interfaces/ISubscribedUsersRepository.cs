using System.Collections.Generic;
using RozkladSubscribeModuleClient.Entities;

namespace RozkladSubscribeModuleClient.Interfaces
{
    internal interface ISubscribedUsersRepository
    {
        void AddUser(SubscribedUser subscribedUser);
        void DeleteUser(SubscribedUser seSubscribedUser);
        List<SubscribedUser> GetUsers();
        SubscribedUser GetUser();
        bool IsUserExists(SubscribedUser subscribedUser);
    }
}