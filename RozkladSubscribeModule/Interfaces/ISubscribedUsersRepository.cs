using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISubscribedUsersRepository
    {
        void AddUser(SubscribedUser subscribedUser);
        void DeleteUser(SubscribedUser subscribedUser);
        List<SubscribedUser> GetUsers();
        SubscribedUser GetUser();
        bool IsUserExists(SubscribedUser subscribedUser);
    }
}