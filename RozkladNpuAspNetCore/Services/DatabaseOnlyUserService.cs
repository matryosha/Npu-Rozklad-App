using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Entities;
using RozkladNpuAspNetCore.Interfaces;
using RozkladNpuAspNetCore.Persistence;

namespace RozkladNpuAspNetCore.Services
{
    public class DatabaseOnlyUserService : IUserService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Dictionary<long, int> _lastMessagesDictionary = new Dictionary<long, int>();
        public DatabaseOnlyUserService(
            IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public async Task AddUser(RozkladUser user)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = 
                    scope.ServiceProvider.GetRequiredService<RozkladBotContext>();
                await dbContext.Users.AddAsync(user)
                    .ConfigureAwait(false);
                await dbContext.SaveChangesAsync();
            }       
        }

        //todo fix fake concurrency 
        public async Task UpdateUser(RozkladUser user)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext =
                    scope.ServiceProvider.GetRequiredService<RozkladBotContext>();
                dbContext.Users.Update(user);
                await dbContext.SaveChangesAsync()
                    .ConfigureAwait(false);
            }
        }

        public async Task<RozkladUser> GetUser(int telegramId)
        {
            RozkladUser user = null;
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext =
                    scope.ServiceProvider.GetRequiredService<RozkladBotContext>();
                user = await dbContext.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
            }

            return user;
        }

        public bool TryGetLastMessageId(long chatId, out int messageId)
        {
            return _lastMessagesDictionary.TryGetValue(chatId, out messageId);
        }

        public void SetLastMessageId(long chatId, int messageId)
        {
            if (!_lastMessagesDictionary.TryAdd(chatId, messageId))
            {
                _lastMessagesDictionary.Remove(chatId);
                _lastMessagesDictionary.Add(chatId, messageId);
            }
        }
    }
}
