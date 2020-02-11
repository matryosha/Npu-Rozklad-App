using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Core.Interfaces
{
    public interface IRozkladProvider
    {
        Task<List<Lesson>> GetLessons();
        Task<List<Group>> GetGroups();
        Task<List<Classroom>> GetClassrooms();
        Task<List<Lecturer>> GetLecturers();
        Task<List<Faculty>> GetFaculties();
    }
}