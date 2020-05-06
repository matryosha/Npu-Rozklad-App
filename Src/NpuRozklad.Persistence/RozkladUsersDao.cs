using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Persistence
{
    public class RozkladUsersDao : IRozkladUsersDao
    {
        private readonly NpuRozkladContext _dbContext;
        private readonly IFacultyGroupsProvider _facultyGroupsProvider;
        private readonly IMemoryCache _memoryCache;

        public RozkladUsersDao(NpuRozkladContext dbContext, IFacultyGroupsProvider facultyGroupsProvider,
            IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _facultyGroupsProvider = facultyGroupsProvider;
            _memoryCache = memoryCache;
        }

        public async Task Add(RozkladUser rozkladUser)
        {
            var alreadyExistedUser =
                await _dbContext.RozkladUserWrappers
                    .AsNoTracking()
                    .Where(u => u.Guid == rozkladUser.Guid)
                    .FirstOrDefaultAsync();

            if (alreadyExistedUser == null)
            {
                var wrapper = new RozkladUserWrapper(rozkladUser);
                _memoryCache.Set(wrapper.Guid, wrapper);
                await _dbContext.RozkladUserWrappers.AddAsync(wrapper);
            }
            else
            {
                alreadyExistedUser.IsDeleted = false;
                _memoryCache.Set(alreadyExistedUser.Guid, alreadyExistedUser);
                _dbContext.RozkladUserWrappers.Update(alreadyExistedUser);
            }
            await _dbContext.SaveChangesAsync();
        }

        public Task Update(RozkladUser rozkladUser)
        {
            var wrapper = new RozkladUserWrapper(rozkladUser);
            _dbContext.RozkladUserWrappers.Update(wrapper);
            _memoryCache.Set(wrapper.Guid, wrapper);
            return _dbContext.SaveChangesAsync();
        }

        public Task Delete(RozkladUser rozkladUser)
        {
            rozkladUser.IsDeleted = true;
            rozkladUser.FacultyGroups.Clear();
            var wrapper = new RozkladUserWrapper(rozkladUser);
            _dbContext.RozkladUserWrappers.Update(wrapper);
            _memoryCache.Remove(wrapper.Guid);
            return _dbContext.SaveChangesAsync();
        }

        public async Task<RozkladUser> Find(string guid)
        {
            if(!_memoryCache.TryGetValue(guid, out RozkladUserWrapper rozkladUserWrapper))
            {
                rozkladUserWrapper =
                    await _dbContext.RozkladUserWrappers
                        .AsNoTracking()
                        .Where(u => u.Guid == guid)
                        .FirstOrDefaultAsync();

                if (rozkladUserWrapper == null) return null;
                var facultyGroups = await _facultyGroupsProvider.GetFacultyGroups();

                foreach (var groupTypeId in rozkladUserWrapper.FacultyGroupsTypeIds)
                {
                    rozkladUserWrapper.FacultyGroups.Add(facultyGroups
                        .FirstOrDefault(g => g.TypeId == groupTypeId));
                }

                _memoryCache.Set(guid, rozkladUserWrapper, absoluteExpirationRelativeToNow: TimeSpan.FromHours(1));
            }
            

            return rozkladUserWrapper;
        }

        public async Task<ICollection<RozkladUser>> GetAll()
        {
            var rozkladUserWrapperList = await _dbContext.RozkladUserWrappers.ToListAsync();
            var facultyGroups = await _facultyGroupsProvider.GetFacultyGroups();

            foreach (var rozkladUserWrapper in rozkladUserWrapperList)
            {
                foreach (var groupTypeId in rozkladUserWrapper.FacultyGroupsTypeIds)
                {
                    rozkladUserWrapper.FacultyGroups.Add(facultyGroups
                        .FirstOrDefault(g => g.TypeId == groupTypeId));
                }
            }

            return new List<RozkladUser>(rozkladUserWrapperList);
        }
    }
}