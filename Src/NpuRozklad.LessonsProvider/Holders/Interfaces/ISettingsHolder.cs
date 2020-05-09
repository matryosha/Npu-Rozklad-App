using System;
using System.Threading.Tasks;

namespace NpuRozklad.LessonsProvider.Holders.Interfaces
{
    internal interface ISettingsHolder
    {
        Task<(DateTime date, bool IsOddDay)> GetSettings();
    }
}