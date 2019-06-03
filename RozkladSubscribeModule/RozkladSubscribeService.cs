using NpuTimetableParser;
using RozkladNpuBot.Domain.Entities;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule
{
    internal class RozkladSubscribeService : IRozkladSubscribeService
    {
        private readonly IBackgroundUsersQueue _usersQueue;
        private readonly ISubscribedUsersRepository _subscribedUsersRepository;

        public RozkladSubscribeService(
            IBackgroundUsersQueue usersQueue,
            ISubscribedUsersRepository subscribedUsersRepository)
        {
            _usersQueue = usersQueue;
            _subscribedUsersRepository = subscribedUsersRepository;
        }

        public void SubscribeUser(RozkladUser user, long chatId, Group group)
        {
            var subscribedUser = 
                new SubscribedUser(user.TelegramId, group.ExternalId, chatId, group.FacultyShortName);
            _usersQueue.QueueNewUser(subscribedUser);
        }

        public void UnsubscribeUser(RozkladUser user, long chatId, Group group)
        {
            _usersQueue.QueueUserToDelete(
                new SubscribedUser(user.TelegramId, group.ExternalId, chatId, group.FacultyShortName));
        }
    }
}