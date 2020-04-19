using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider
{
    public class FacultyGroupsProvider : IFacultyGroupsProvider
    {
        private readonly IGroupsHolder _groupsHolder;

        public FacultyGroupsProvider(IGroupsHolder groupsHolder)
        {
            _groupsHolder = groupsHolder;
        }
        
        public async Task<ICollection<Group>> GetFacultyGroups(Faculty faculty)
        {
            var allGroups = await _groupsHolder.GetFacultiesGroups().ConfigureAwait(false);
            return allGroups.Where(g => g.Faculty.Equals(faculty)).ToList();
        }
    }
}