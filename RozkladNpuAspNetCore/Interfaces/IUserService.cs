using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RozkladNpuAspNetCore.Entities;

namespace RozkladNpuAspNetCore.Interfaces
{
    public interface IUserService
    {
        Task AddUser(RozkladUser user);

        Task UpdateUser(RozkladUser user);

        Task<RozkladUser.LastActionType> GetUserLastAction(string guid);

        Task<RozkladUser> GetUser(int telegramId);

    }
}
