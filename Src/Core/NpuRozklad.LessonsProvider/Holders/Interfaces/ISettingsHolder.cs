using System;
using System.Threading.Tasks;

namespace NpuRozklad.LessonsProvider.Holders.Interfaces
{
    public interface ISettingsHolder
    {
        Task<(DateTime date, bool IsOddDay)> GetSettings();
    }
}