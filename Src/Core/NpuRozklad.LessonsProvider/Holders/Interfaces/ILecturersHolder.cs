using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.LessonsProvider.Holders.Interfaces
{
    public interface ILecturersHolder
    {
        Task<ICollection<Lecturer>> GetLecturers(Faculty faculty);
    }
}