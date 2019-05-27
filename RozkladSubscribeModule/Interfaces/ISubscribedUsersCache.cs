using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISubscribedUsersCache
    {
        void AddUser (SubscribedUser user);
        void RemoveUser(SubscribedUser user);
        List<SubscribedUser> GetUsers(SubscribedUser subscribedUser);
        bool IsUserExists(SubscribedUser subscribedUser);
    } 
}