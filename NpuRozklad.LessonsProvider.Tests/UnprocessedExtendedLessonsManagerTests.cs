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
        // private UnprocessedExtendedLessonsManager _manager;
        
        [SetUp]
        public void Setup()
        {
            // var groupsHolderStub = CreateIGroupsHolderStub();
            // var lecturerHolderStub = CreateILecturersHolder();
            // var classroomsHolderStub = CreateIClassroomsHolder();
            // var calendarRawItemHolderStub = CreateICalendarRawItemHolder();
            //
            //  _manager = new UnprocessedExtendedLessonsManager(
            //      groupsHolderStub, lecturerHolderStub, classroomsHolderStub, calendarRawItemHolderStub);
        }
        
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(101)]
        [TestCase(102)]
        [TestCase(103)]
        [TestCase(104)]
        [TestCase(105)]
        [TestCase(106)]
        [TestCase(107)]
        [TestCase(108)]
        [TestCase(109)]
        [TestCase(110)]
        [TestCase(111)]
        [TestCase(112)]
        [TestCase(113)]
        [TestCase(114)]
        [TestCase(115)]
        [TestCase(116)]
        [TestCase(117)]
        [TestCase(118)]
        [TestCase(119)]
        [TestCase(120)]
        [TestCase(121)]
        [TestCase(122)]
        [TestCase(123)]
        [TestCase(124)]
        [TestCase(125)]
        [TestCase(126)]
        [TestCase(127)]
        [TestCase(128)]
        [TestCase(129)]
        [TestCase(130)]
        [TestCase(131)]
        [TestCase(132)]
        [TestCase(133)]
        [TestCase(134)]
        [TestCase(135)]
        [TestCase(136)]
        [TestCase(137)]
        [TestCase(138)]
        [TestCase(139)]
        [TestCase(140)]
        [TestCase(141)]
        [TestCase(142)]
        [TestCase(143)]
        [TestCase(144)]
        [TestCase(145)]
        [TestCase(146)]
        [TestCase(147)]
        [TestCase(148)]
        [TestCase(149)]
        [TestCase(150)]
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
        [TestCase(102)]
        [TestCase(103)]
        [TestCase(104)]
        [TestCase(105)]
        [TestCase(106)]
        [TestCase(107)]
        [TestCase(108)]
        [TestCase(109)]
        [TestCase(110)]
        [TestCase(111)]
        [TestCase(112)]
        [TestCase(113)]
        [TestCase(114)]
        [TestCase(115)]
        [TestCase(116)]
        [TestCase(117)]
        [TestCase(118)]
        [TestCase(119)]
        [TestCase(120)]
        [TestCase(121)]
        [TestCase(122)]
        [TestCase(123)]
        [TestCase(124)]
        [TestCase(125)]
        [TestCase(126)]
        [TestCase(127)]
        [TestCase(128)]
        [TestCase(129)]
        [TestCase(130)]
        [TestCase(131)]
        [TestCase(132)]
        [TestCase(133)]
        [TestCase(134)]
        [TestCase(135)]
        [TestCase(136)]
        [TestCase(137)]
        [TestCase(138)]
        [TestCase(139)]
        [TestCase(140)]
        [TestCase(141)]
        [TestCase(142)]
        [TestCase(143)]
        [TestCase(144)]
        [TestCase(145)]
        [TestCase(146)]
        [TestCase(147)]
        [TestCase(148)]
        [TestCase(149)]
        [TestCase(150)]
        [Timeout(10000)]
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
        
        private ICalendarRawItemHolder CreateICalendarRawItemHolder()
        {
            var stub = new Mock<ICalendarRawItemHolder>();
            stub.Setup(g => g.GetCalendarItems())
                .ReturnsAsync(new List<CalendarRawItem>());
            return stub.Object;
        }
        
    }
}