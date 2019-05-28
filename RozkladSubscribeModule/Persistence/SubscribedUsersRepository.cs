using System.Collections.Concurrent;
using System.Collections.Generic;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Persistence
{
    internal class SubscribedUsersRepository :
        ISubscribedUsersRepository
    {
        private readonly ISubscribedUsersCache _cache;

        private readonly HashSet<SubscribedUser> _subscribedUsers;



        public SubscribedUsersRepository(
            ISubscribedUsersCache cache)
        {
            _cache = cache;
            _subscribedUsers = 
                new HashSet<SubscribedUser>();
        }
        public void AddUser(SubscribedUser subscribedUser)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteUser(SubscribedUser seSubscribedUser)
        {
            throw new System.NotImplementedException();
        }

        public List<SubscribedUser> GetUsers()
        {
            return _cache.GetUsers();
        }

        public SubscribedUser GetUser()
        {
            throw new System.NotImplementedException();
        }

        public bool IsUserExists(SubscribedUser subscribedUser)
        {
            throw new System.NotImplementedException();
        }
    }
}