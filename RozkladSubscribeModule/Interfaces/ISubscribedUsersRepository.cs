using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
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