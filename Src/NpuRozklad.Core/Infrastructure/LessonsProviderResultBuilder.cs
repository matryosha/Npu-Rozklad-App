using System.Collections.Generic;
using NpuRozklad.Core.Entities;

namespace NpuRozklad.Core.Infrastructure
{
    public class LessonsProviderResultBuilder
    {
        private readonly ICollection<Lesson> _lessons = new List<Lesson>();
        private bool _isNpuServerOffline;
        private bool _isErrorOccured;
        
        public LessonsProviderResultBuilder AddLessons(ICollection<Lesson> lessons)
        {
            foreach (var lesson in lessons)
            {
                _lessons.Add(lesson);
            }
            
            return this;
        }

        public LessonsProviderResultBuilder CheckErrorOccured()
        {
            _isErrorOccured = true;
            return this;
        }

        public LessonsProviderResultBuilder CheckNpuServerOffline()
        {
            _isNpuServerOffline = true;
            return this;
        }
        
        public LessonsProviderResult Build()
        {
            return new LessonsProviderResult((IReadOnlyCollection<Lesson>) _lessons, _isNpuServerOffline, _isErrorOccured);
        }
    }
}