using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.LessonsProvider.Entities;

namespace NpuRozklad.LessonsProvider.Holders.Interfaces
{
    internal interface IUnprocessedExtendedLessonsHolder
    {
        Task<ICollection<ExtendedLesson>> GetFacultyUnprocessedLessons(Faculty faculty);
    }
}