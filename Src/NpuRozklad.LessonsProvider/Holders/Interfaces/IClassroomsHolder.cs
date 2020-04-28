using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.LessonsProvider.Holders.Interfaces
{
    public interface IClassroomsHolder
    {
        Task<ICollection<Classroom>> GetClassrooms(Faculty faculty);
    }
}