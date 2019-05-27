using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISubscribedUsersCache
    {
        SubscribedUser AddUser();
        void RemoveUser(SubscribedUser user);
        List<SubscribedUser> GetUsers(SubscribedUser subscribedUser);
        bool IsUserExists(SubscribedUser subscribedUser);
    } 
}