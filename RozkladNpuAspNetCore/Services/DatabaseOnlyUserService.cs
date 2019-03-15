using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RozkladNpuAspNetCore.Entities;
using RozkladNpuAspNetCore.Interfaces;
using RozkladNpuAspNetCore.Persistence;

namespace RozkladNpuAspNetCore.Services
{
    public class DatabaseOnlyUserService : IUserService
    {
        private readonly RozkladBotContext _context;

        public DatabaseOnlyUserService(
            RozkladBotContext context)
        {
            _context = context;
        }
        public Task AddUser(RozkladUser user) => _context.Users.AddAsync(user);

        public void UpdateUser(RozkladUser user) => _context.Users.Update(user);

        public Task<RozkladUser> GetUser(int telegramId) => _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
    }
}
