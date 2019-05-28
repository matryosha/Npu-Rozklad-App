using System.Collections.Generic;
using System.Threading.Tasks;
using RozkladSubscribeModule.Entities;

namespace RozkladSubscribeModule.Interfaces
{
    internal interface ISubscribedUsersRepository
    {
        Task AddUserAsync(SubscribedUser subscribedUser);
        Task DeleteUserAsync(SubscribedUser subscribedUser);
        Task<ICollection<SubscribedUser>> GetUsersAsync();
        SubscribedUser GetUser();
        Task<bool> IsUserExistsAsync(SubscribedUser subscribedUser);
    }
}