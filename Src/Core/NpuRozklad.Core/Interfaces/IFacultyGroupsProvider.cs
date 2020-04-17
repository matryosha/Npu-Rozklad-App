using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Core.Interfaces
{
    public interface IFacultyGroupsProvider
    {
        Task<ICollection<Group>> GetFacultyGroups(Faculty faculty);
    }
}