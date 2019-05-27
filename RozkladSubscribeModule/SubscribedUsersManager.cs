using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RozkladNpuAspNetCore.Persistence;
using RozkladSubscribeModuleClient.Entities;
using RozkladSubscribeModuleClient.Interfaces;

namespace RozkladSubscribeModuleClient
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
