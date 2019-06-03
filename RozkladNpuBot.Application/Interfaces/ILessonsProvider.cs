using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpuTimetableParser;

namespace RozkladNpuBot.Application.Interfaces
{
    public interface ILessonsProvider
    {
        List<Faculty> GetFaculties();
        Task<List<Group>> GetGroups(string facultyShortName);
        Task<List<Lesson>> GetLessonsOnDate(string facultyShortName, int groupId, DateTime date);
    }
}
