using System.Collections.Generic;
using RozkladSubscribeModuleClient.Entities;

namespace RozkladSubscribeModuleClient.Interfaces
{
    internal interface ISubscribedUsersCache
    {
        SubscribedUser AddUser();
        void RemoveUser(SubscribedUser user);
        List<SubscribedUser> GetUsers(SubscribedUser subscribedUser);
        bool IsUserExists(SubscribedUser subscribedUser);
    } 
}