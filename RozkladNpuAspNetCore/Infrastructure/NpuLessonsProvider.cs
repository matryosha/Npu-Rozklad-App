using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpuTimetableParser;
using RozkladNpuAspNetCore.Interfaces;

namespace RozkladNpuAspNetCore.Infrastructure
{
    public class NpuLessonsProvider : ILessonsProvider
    {
        private readonly NpuParser _provider;
        public NpuLessonsProvider()
        {
            _provider = new NpuParser();
        }
        public List<Faculty> GetFaculties() => _provider.GetFaculties();

        public async Task<List<Group>> GetGroups(string facultyShortName)
        {
             return await _provider.GetGroups(facultyShortName);
        }

        public async Task<List<Lesson>> GetLessonsOnDate(string facultyShortName, int groupId, DateTime date)
        {
            return await _provider.GetLessonsOnDate(facultyShortName, groupId, date);
        }
    }
}
