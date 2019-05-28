using System.Collections.Generic;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISubscribedUsersCache :
        ISubscribedUsersRepository
    {
        void SetUsers(ICollection<SubscribedUser> users);
    } 
}