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
                var saved = false;
                while (!saved)
                {
                    try
                    {
                        await dbContext.SaveChangesAsync()
                            .ConfigureAwait(false);
                        saved = true;
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        foreach (var entry in e.Entries)
                        {
                            if (entry.Entity is Group)
                            {
                                var proposedValues = entry.CurrentValues;
                                var databaseValues = entry.GetDatabaseValues();

                                if (databaseValues == null)
                                {
                                    entry.Da.SetValues(proposedValues);
                                    break;
                                }
                                foreach (var property in proposedValues.Properties)
                                {
                                    var proposedValue = proposedValues[property];
                                    var databaseValue = databaseValues[property];

                                    // TODO: decide which value should be written to database
                                    // proposedValues[property] = <value to be saved>;
                                }

                                // Refresh original values to bypass next concurrency check
                                entry.OriginalValues.SetValues(databaseValues);
                            }
                            else
                            {
                                throw new NotSupportedException(
                                    "Don't know how to handle concurrency conflicts for "
                                    + entry.Metadata.Name);
                            }
                        }
                    }
                }
            }
        }

        public async Task<RozkladUser> GetUser(int telegramId)
        {
            RozkladUser user = null;
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext =
                    scope.ServiceProvider.GetRequiredService<RozkladBotContext>();
                user =  await dbContext.Users.Where(u => u.TelegramId == telegramId)
                    .Include(u => u.Groups)
                    .FirstOrDefaultAsync();
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
