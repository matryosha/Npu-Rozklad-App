using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;

namespace NpuRozklad.Persistence
{
    public class RozkladUsersDao : IRozkladUsersDao
    {
        private readonly NpuRozkladContext _dbContext;
        private readonly IFacultyGroupsProvider _facultyGroupsProvider;

        public RozkladUsersDao(NpuRozkladContext dbContext, IFacultyGroupsProvider facultyGroupsProvider)
        {
            _dbContext = dbContext;
            _facultyGroupsProvider = facultyGroupsProvider;
        }
        
        public async Task Add(RozkladUser rozkladUser)
        {
            var alreadyExistedUser = 
                await _dbContext.RozkladUserWrappers.FindAsync(rozkladUser.Guid);

            if (alreadyExistedUser == null)
            {
                var wrapper = new RozkladUserWrapper(rozkladUser);
                await _dbContext.RozkladUserWrappers.AddAsync(wrapper);
            }
            else
            {
                alreadyExistedUser.IsDeleted = false;
            }
            
            await _dbContext.SaveChangesAsync();
        }

        public Task Update(RozkladUser rozkladUser)
        {
            var wrapper = new RozkladUserWrapper(rozkladUser);
            _dbContext.RozkladUserWrappers.Update(wrapper);
            return _dbContext.SaveChangesAsync();
        }

        public Task Delete(RozkladUser rozkladUser)
        {
            rozkladUser.IsDeleted = true;
            rozkladUser.FacultyGroups.Clear();
            var wrapper = new RozkladUserWrapper(rozkladUser);
            _dbContext.RozkladUserWrappers.Update(wrapper);
            return _dbContext.SaveChangesAsync();
        }

        public async Task<RozkladUser> Find(string guid)
        {
            var rozkladUserWrapper = await _dbContext.RozkladUserWrappers.FindAsync(guid);
            if (rozkladUserWrapper == null) return null;
            var facultyGroups = await _facultyGroupsProvider.GetFacultyGroups();

            foreach (var groupTypeId in rozkladUserWrapper.FacultyGroupsTypeIds)
            {
                rozkladUserWrapper.FacultyGroups.Add(facultyGroups
                    .FirstOrDefault(g => g.TypeId == groupTypeId));
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