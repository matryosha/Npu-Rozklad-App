using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NpuRozklad.Telegram.LongLastingUserActions;
using NUnit.Framework;

namespace NpuRozklad.Telegram.Tests
{
    public class LongLastingUserActionManagerTests
    {
        private LongLastingUserActionManager _manager;

        [SetUp]
        public void Setup()
        {
            _manager = new LongLastingUserActionManager(-1);
        }
        
        [TestCase(1_000)]
        [TestCase(10_000)]
        public async Task LongLastingUserActionManagerMultiThreadStressTest(int userCount)
        {
            var users = GetDifferentUsers(userCount);
            var usersLongLastingActionArguments = new LongLastingUserActionArguments[userCount];
            var random = new Random();
            var eachTasksNumber = userCount * 10;

            for (var i = 0; i < userCount; i++)
            {
                usersLongLastingActionArguments[i] = GetUserActionArguments(users[i]);
            }

            var upsertingTasks = new Task[eachTasksNumber];
            Parallel.For(0, eachTasksNumber, i =>
            {
                var task = Task.Run(async () =>
                {
                    await Task.Delay(random.Next(100, 3000));
                    await _manager.UpsertUserAction(usersLongLastingActionArguments[random.Next(0, userCount)]);
                });
                upsertingTasks[i]= task;
            });
            
            var gettingUserActionArgsTasks = new Task[eachTasksNumber];

            Parallel.For(0, eachTasksNumber, i =>
            {
                var task = Task.Run(async () =>
                {
                    await Task.Delay(random.Next(100, 3000));
                    await _manager.GetUserLongLastingAction(users[random.Next(0, userCount)]);
                });

                gettingUserActionArgsTasks[i] = task;
            });
            
            var clearingUserActionArgsTasks = new Task[eachTasksNumber];

            Parallel.For(0, eachTasksNumber, i =>
            {
                var task = Task.Run(async () =>
                {
                    await Task.Delay(random.Next(100, 3000));
                    await _manager.ClearUserAction(users[random.Next(0, userCount)]);
                });

                clearingUserActionArgsTasks[i] = task;
            });
            
            var clearOldValuesTasks = new Task[eachTasksNumber];

            Parallel.For(0, eachTasksNumber, i =>
            {
                var task = Task.Run(async () =>
                {
                    await Task.Delay(random.Next(100, 1000));
                    await _manager.ClearOldValues();
                });

                clearOldValuesTasks[i] = task;
            });

            var tasksList = new List<Task[]>();
            tasksList.Add(upsertingTasks);
            tasksList.Add(gettingUserActionArgsTasks);
            tasksList.Add(clearingUserActionArgsTasks);
            tasksList.Add(clearOldValuesTasks);

            var allTasksArray = new Task[eachTasksNumber * 4];

            long multiplier = 0;
            foreach (var t in tasksList)
            {
                for (long j = 0; j < eachTasksNumber; j++)
                {
                    allTasksArray[j + multiplier] = t[j];
                }

                multiplier += eachTasksNumber;
            }

            // Parallel.For(0, allTasksArray.Length, i => allTasksArray[i].Start());

            await Task.WhenAll(allTasksArray);
        }

        public TelegramRozkladUser[] GetDifferentUsers(long count)
        {
            var result = new TelegramRozkladUser[count];
            for (var i = 0; i < count; i++)
            {
                result[i] = new TelegramRozkladUser();
            }

            return result;
        }

        private LongLastingUserActionArguments GetUserActionArguments(TelegramRozkladUser user)
        {
            return new LongLastingUserActionArguments
            {
                TelegramRozkladUser = user
            };
        }
    }
}