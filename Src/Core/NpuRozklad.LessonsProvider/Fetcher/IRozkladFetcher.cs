using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Parser.Fetcher
{
    public interface IRozkladFetcher
    {
        Task<List<Lesson>> FetchLessons();
        Task<List<Group>> FetchGroups();
        Task<List<Classroom>> FetchClassrooms();
        Task<List<Lecturer>> FetchLecturers();
        Task<List<Faculty>> FetchFaculties();
    }
}