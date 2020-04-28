using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Infrastructure;

namespace NpuRozklad.Telegram.LongLastingUserActions
{
    public class LongLastingUserActionManager : PeriodicOperationExecutor, ILongLastingUserActionManager
    {
        private readonly ConcurrentDictionary<TelegramRozkladUser, ArgumentsWrapper> _actionCache =
            new ConcurrentDictionary<TelegramRozkladUser, ArgumentsWrapper>();
        
        public LongLastingUserActionManager(int cacheTimeClearInMinutes = 5)
        {
            PeriodicCallIntervalInSeconds = cacheTimeClearInMinutes;
            PeriodicAction = ClearOldValues;
        }

        public Task UpsertUserAction(LongLastingUserActionArguments arguments)
        {
            var wrapper = new ArgumentsWrapper
            {
                AddDate = DateTimeOffset.UtcNow,
                Arguments = arguments
            };

            _actionCache[arguments.TelegramRozkladUser] = wrapper;
            
            return Task.CompletedTask;
        }

        public Task ClearUserAction(TelegramRozkladUser rozkladUser)
        {
            _actionCache.TryRemove(rozkladUser, out _);
            return Task.CompletedTask;
        }

        public Task<LongLastingUserActionArguments> GetUserLongLastingAction(TelegramRozkladUser telegramRozkladUser)
        {
            return _actionCache.TryGetValue(telegramRozkladUser, out var value)
                ? Task.FromResult(value.Arguments)
                : Task.FromResult<LongLastingUserActionArguments>(null);
        }

        public Task ClearOldValues()
        {
            var allValues = new Dictionary<TelegramRozkladUser, ArgumentsWrapper>(_actionCache);
            var usersToClear = new List<TelegramRozkladUser>();
            var currentDate = DateTimeOffset.UtcNow;

            foreach (var value in allValues)
            {
                var argumentDate = value.Value.AddDate;
                var timeSpan = argumentDate.Subtract(currentDate);
                
                if (timeSpan.TotalSeconds > PeriodicCallIntervalInSeconds)
                    usersToClear.Add(value.Key);
            }

            foreach (var user in usersToClear)
            {
                _actionCache.TryRemove(user, out _);
            }
            
            return Task.CompletedTask;
        }

        private class ArgumentsWrapper
        {
            public DateTimeOffset AddDate { get; set; }
            public LongLastingUserActionArguments Arguments { get; set; }
        }
    }
}