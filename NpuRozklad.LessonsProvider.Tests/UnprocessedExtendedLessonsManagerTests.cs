using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NpuRozklad.Core.Entities;
using NpuRozklad.LessonsProvider.Entities;
using NpuRozklad.LessonsProvider.Holders.Interfaces;
using NUnit.Framework;

namespace NpuRozklad.LessonsProvider.Tests
{
    public class UnprocessedExtendedLessonsManagerTests
    {
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [Timeout(60000)]
        public async Task MultipleSameFacultyLessonsRequest_FirstRun_ShouldFetchCalendarItemsOnlyOnce(int taskCount)
        {
            var random = new Random();
            var getCalendarItemsCallCount = 0;
            var resultGotCount = 0;
            var calendarRawItemHolderStub = new Mock<ICalendarRawItemHolder>();
            calendarRawItemHolderStub.Setup(g => g.GetCalendarItems())
                .Callback(() => ++getCalendarItemsCallCount)
                .Returns(GetCalendarList);
            
            var manager = new UnprocessedExtendedLessonsManager(
                CreateIGroupsHolderStub(),
                CreateILecturersHolder(),
                CreateIClassroomsHolder(),
                calendarRawItemHolderStub.Object);

            var fetchTasks = new List<Task>();
            
            Parallel.For(0, taskCount, 
                it => 
                    fetchTasks.Add(CallTaskWithRandomDelay()));

            foreach (var task in fetchTasks)
            {
                
            }
            var t =  Task.WhenAll(fetchTasks.ToList());
            await t;
            Assert.AreEqual(1, getCalendarItemsCallCount);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status);

            Task CallTaskWithRandomDelay()
            {
                return Task.Delay(100 * random.Next(0, 10)).ContinueWith(async _ => await manager.GetFacultyUnprocessedLessons(new Faculty("asd", "asd")));
            }
            
        }
        
        
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(101)]
        [Timeout(60000)]
        public async Task MultipleSeveralFacultyLessonsRequests_FirstRun_ShouldFetchCalendarItemsOnlyOnce(int taskCount)
        {
            var getCalendarItemsCallCount = 0;
            var resultGotCount = 0;
            var calendarRawItemHolderStub = new Mock<ICalendarRawItemHolder>();
            calendarRawItemHolderStub.Setup(g => g.GetCalendarItems())
                .Callback(() => ++getCalendarItemsCallCount)
                .Returns(GetCalendarList);

            var manager = new UnprocessedExtendedLessonsManager(
                CreateIGroupsHolderStub(),
                CreateILecturersHolder(),
                CreateIClassroomsHolder(),
                calendarRawItemHolderStub.Object);

            var fetchTasks = new List<Task>();

            Parallel.For(0, taskCount / 3,
                it =>
                    fetchTasks.Add(manager.GetFacultyUnprocessedLessons(new Faculty("asd", "asd"))));

            Parallel.For(0, taskCount / 3,
                it =>
                    fetchTasks.Add(manager.GetFacultyUnprocessedLessons(new Faculty("dsa", "dsa"))));

            Parallel.For(0, taskCount / 3,
                it =>
                    fetchTasks.Add(manager.GetFacultyUnprocessedLessons(new Faculty("123", "dsa"))));

            foreach (var task in fetchTasks)
            {
            }

            var t = Task.WhenAll(fetchTasks.ToList());
            await t;
            Assert.AreEqual(3, getCalendarItemsCallCount);
            Assert.AreEqual(TaskStatus.RanToCompletion, t.Status);
        }

        private async Task<ICollection<CalendarRawItem>> GetCalendarList()
        {
            await Task.Delay(100);
            return new List<CalendarRawItem>();
        }
        private IGroupsHolder CreateIGroupsHolderStub()
        {
            var stub = new Mock<IGroupsHolder>();
            stub.Setup(g => g.GetFacultiesGroups()).ReturnsAsync(new List<Group>());
            return stub.Object;
        }

        private ILecturersHolder CreateILecturersHolder()
        {
            var stub = new Mock<ILecturersHolder>();
            stub.Setup(g => g.GetLecturers(It.IsAny<Faculty>()))
                .ReturnsAsync(new List<Lecturer>());
            return stub.Object;
        }
        
        private IClassroomsHolder CreateIClassroomsHolder()
        {
            var stub = new Mock<IClassroomsHolder>();
            stub.Setup(g => g.GetClassrooms(It.IsAny<Faculty>()))
                .ReturnsAsync(new List<Classroom>());
            return stub.Object;
        }
    }
}