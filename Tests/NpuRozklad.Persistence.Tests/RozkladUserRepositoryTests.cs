using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NUnit.Framework;

namespace NpuRozklad.Persistence.Tests
{
    public class RozkladUserRepositoryTests
    {
        private static readonly IMemoryCache StubCache = GetStubCache();
        [Test]
        public async Task Add__Adding_Not_Existed_RozkladUser()
        {
            var expectedRozkladUser = new RozkladUser();
            expectedRozkladUser.FacultyGroups.AddRange(GetMockGroupList1());
            expectedRozkladUser.IsDeleted = false;


            await using (var context = new NpuRozkladContext(GetContextOptions("add1")))
            {
                var repo = new RozkladUsersDao(context, Mock.Of<IFacultyGroupsProvider>(), StubCache);

                await repo.Add(expectedRozkladUser);
            }


            RozkladUserWrapper actualRozkladUserWrapper;

            await using (var context = new NpuRozkladContext(GetContextOptions("add1")))
            {
                actualRozkladUserWrapper = await context.RozkladUserWrappers.SingleAsync();
            }

            Assert.AreEqual(expectedRozkladUser.Guid, actualRozkladUserWrapper.Guid);
            Assert.AreEqual(expectedRozkladUser.IsDeleted, actualRozkladUserWrapper.IsDeleted);
            Assert.AreEqual(expectedRozkladUser.FacultyGroups.Count, actualRozkladUserWrapper.FacultyGroupsTypeIds.Count);

            foreach (var group in expectedRozkladUser.FacultyGroups)
            {
                Assert.Contains(group.TypeId, actualRozkladUserWrapper.FacultyGroupsTypeIds);
            }
        }

        [Test]
        public async Task Add__Adding_Existed_Before_But_Was_Mark_Deleted_RozkladUser()
        {
            var expectedRozkladUser = new RozkladUser();
            expectedRozkladUser.FacultyGroups.AddRange(GetMockGroupList1());
            expectedRozkladUser.IsDeleted = true;

            await using (var context = new NpuRozkladContext(GetContextOptions("add2")))
            {
                await context.AddAsync(new RozkladUserWrapper(expectedRozkladUser));
                await context.SaveChangesAsync();
            }

            await using (var context = new NpuRozkladContext(GetContextOptions("add2")))
            {
                var repo = new RozkladUsersDao(context, Mock.Of<IFacultyGroupsProvider>(), StubCache);

                await repo.Add(expectedRozkladUser);
            }

            RozkladUserWrapper actualRozkladUserWrapper;

            await using (var context = new NpuRozkladContext(GetContextOptions("add2")))
            {
                actualRozkladUserWrapper = await context.RozkladUserWrappers.SingleAsync();
                Assert.AreEqual(1, context.RozkladUserWrappers.Count());
            }

            Assert.AreEqual(expectedRozkladUser.Guid, actualRozkladUserWrapper.Guid);
            Assert.False(actualRozkladUserWrapper.IsDeleted);
            Assert.AreEqual(expectedRozkladUser.FacultyGroups.Count, actualRozkladUserWrapper.FacultyGroupsTypeIds.Count);

            foreach (var group in expectedRozkladUser.FacultyGroups)
            {
                Assert.Contains(group.TypeId, actualRozkladUserWrapper.FacultyGroupsTypeIds);
            }
        }


        [Test]
        public async Task Update__Updating_Existed_RozkladUser()
        {
            var expectedRozkladUser = new RozkladUser();
            expectedRozkladUser.FacultyGroups.AddRange(GetMockGroupList1());
            expectedRozkladUser.IsDeleted = false;

            await using (var context = new NpuRozkladContext(GetContextOptions("update1")))
            {
                await context.AddAsync(new RozkladUserWrapper(expectedRozkladUser));
                await context.SaveChangesAsync();
            }

            await using (var context = new NpuRozkladContext(GetContextOptions("update1")))
            {
                var repo = new RozkladUsersDao(context, Mock.Of<IFacultyGroupsProvider>(), StubCache);

                expectedRozkladUser.FacultyGroups.Add(new Group("c", "C", expectedRozkladUser.FacultyGroups.First().Faculty));
                await repo.Update(expectedRozkladUser);
            }

            RozkladUserWrapper actualRozkladUserWrapper;

            await using (var context = new NpuRozkladContext(GetContextOptions("update1")))
            {
                actualRozkladUserWrapper = await context.RozkladUserWrappers.SingleAsync();
                Assert.AreEqual(1, context.RozkladUserWrappers.Count());
            }

            Assert.AreEqual(expectedRozkladUser.Guid, actualRozkladUserWrapper.Guid);
            Assert.False(actualRozkladUserWrapper.IsDeleted);
            Assert.AreEqual(expectedRozkladUser.FacultyGroups.Count, actualRozkladUserWrapper.FacultyGroupsTypeIds.Count);

            foreach (var group in expectedRozkladUser.FacultyGroups)
            {
                Assert.Contains(group.TypeId, actualRozkladUserWrapper.FacultyGroupsTypeIds);
            }
        }

        [Test]
        public async Task Delete__Deleting_Existing_RozkladUser__Should_Change_IsDeleted_Property()
        {
            var expectedRozkladUser = new RozkladUser();
            expectedRozkladUser.FacultyGroups.AddRange(GetMockGroupList1());
            expectedRozkladUser.IsDeleted = false;

            await using (var context = new NpuRozkladContext(GetContextOptions("delete1")))
            {
                await context.AddAsync(new RozkladUserWrapper(expectedRozkladUser));
                await context.SaveChangesAsync();
            }

            await using (var context = new NpuRozkladContext(GetContextOptions("delete1")))
            {
                var repo = new RozkladUsersDao(context, Mock.Of<IFacultyGroupsProvider>(), StubCache);
                await repo.Delete(expectedRozkladUser);
            }

            RozkladUserWrapper actualRozkladUserWrapper;

            await using (var context = new NpuRozkladContext(GetContextOptions("delete1")))
            {
                actualRozkladUserWrapper = await context.RozkladUserWrappers.SingleAsync();
                Assert.AreEqual(1, context.RozkladUserWrappers.Count());
            }

            Assert.AreEqual(expectedRozkladUser.Guid, actualRozkladUserWrapper.Guid);
            Assert.True(actualRozkladUserWrapper.IsDeleted);
            Assert.AreEqual(0, actualRozkladUserWrapper.FacultyGroupsTypeIds.Count);
        }

        [Test]
        public async Task Find__Should_Find_RozkladUser_And_Fill_Groups_List_From_String()
        {
            var expectedRozkladUser = new RozkladUser();
            expectedRozkladUser.FacultyGroups.AddRange(GetMockGroupList1());
            expectedRozkladUser.IsDeleted = false;

            await using (var context = new NpuRozkladContext(GetContextOptions("find1")))
            {
                await context.AddAsync(new RozkladUserWrapper(expectedRozkladUser));
                await context.SaveChangesAsync();
            }

            var providerMock = new Mock<IFacultyGroupsProvider>();
            providerMock.Setup(p => p.GetFacultyGroups())
                .ReturnsAsync(GetMockGroupList1());

            RozkladUser actualRozkladUser = null;
            await using (var context = new NpuRozkladContext(GetContextOptions("find1")))
            {
                var repo = new RozkladUsersDao(context, providerMock.Object, StubCache);
                actualRozkladUser = await repo.Find(expectedRozkladUser.Guid);
            }

            Assert.NotNull(actualRozkladUser);
            Assert.AreEqual(expectedRozkladUser.Guid, actualRozkladUser.Guid);
            Assert.AreEqual(expectedRozkladUser.IsDeleted, actualRozkladUser.IsDeleted);

            foreach (var expectedFacultyGroup in expectedRozkladUser.FacultyGroups)
            {
                var actualFacultyGroup =
                    actualRozkladUser.FacultyGroups.FirstOrDefault(
                        g => g.TypeId == expectedFacultyGroup.TypeId);
                
                Assert.NotNull(actualRozkladUser);
                Assert.AreEqual(expectedFacultyGroup.Faculty, actualFacultyGroup.Faculty);
                Assert.AreEqual(expectedFacultyGroup.Name, actualFacultyGroup.Name);
            }
        }

        private List<Group> GetMockGroupList1()
        {
            var faculty = new Faculty("test", "TEST");
            var groupA = new Group("a", "A", faculty);
            var groupB = new Group("b", "B", faculty);

            return new List<Group> {groupA, groupB};
        }

        private DbContextOptions<NpuRozkladContext> GetContextOptions(string dbName) =>
            new DbContextOptionsBuilder<NpuRozkladContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        private static IMemoryCache GetStubCache()
        {
            var mock = new Mock<IMemoryCache>();
            
            mock.Setup(cache => cache.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
            return mock.Object;
        }
    }
}