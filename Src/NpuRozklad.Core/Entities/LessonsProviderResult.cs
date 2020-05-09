using System.Collections.Generic;

namespace NpuRozklad.Core.Entities
{
    public class LessonsProviderResult
    {
        public LessonsProviderResult(IReadOnlyCollection<Lesson> lessons, bool isNpuServerOffline = false, bool isErrorOccured = false)
        {
            Lessons = lessons;
            IsNpuServerOffline = isNpuServerOffline;
            IsErrorOccured = isErrorOccured;
        }

        public IReadOnlyCollection<Lesson> Lessons { get; }
        public bool IsNpuServerOffline { get; }
        public bool IsErrorOccured { get; }
    }
}