using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule
{
    internal class SubscribedUsersManager : ISubscribedUsersManager
    {
        public SubscribedUsersManager(
             )
        {
            
        }

        public Task AddUser(int telegramId, int groupExternalId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUser(int telegramId, int groupExternalId)
        {
            throw new NotImplementedException();
        }

        public List<SubscribedUser> GetUsers()
        {
            throw new NotImplementedException();
        }
    }
}
