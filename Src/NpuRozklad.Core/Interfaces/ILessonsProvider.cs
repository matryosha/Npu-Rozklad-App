using System;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Core.Interfaces
{
    public interface ILessonsProvider
    {
        Task<LessonsProviderResult> GetLessonsOnDate(Group facultyGroup, DateTime date);
    }
}