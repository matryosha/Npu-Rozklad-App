using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Core.Interfaces
{
    public interface ILessonsProvider
    {
        Task<ICollection<Lesson>> GetLessonsOnDate(Group facultyGroup, DateTime date);
    }
}