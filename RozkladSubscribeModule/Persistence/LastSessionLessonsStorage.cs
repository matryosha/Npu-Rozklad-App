using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using RozkladSubscribeModule.Entities;
using RozkladSubscribeModule.Interfaces;

namespace RozkladSubscribeModule.Persistence
{
    internal class LastSessionLessonsStorage :
        ILastSessionLessonsStorage
    {
        private readonly MemoryCache _cache;
        public LastSessionLessonsStorage()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }
        public SectionLessons Get(List<DateTime> dateTimes, string facultyShortName, int groupExternalId)
        {
            if (_cache.TryGetValue((facultyShortName, groupExternalId), out var result))
                return result as SectionLessons;

            return null;
        }

        public void Set(List<DateTime> dateTimes,
            string facultyShortName,
            int groupExternalId,
            SectionLessons sectionLessons)
        {
            if(_cache.TryGetValue((facultyShortName, groupExternalId), out var result))
                _cache.Remove((facultyShortName, groupExternalId));
            _cache.Set((facultyShortName, groupExternalId), sectionLessons,
                TimeSpan.FromMinutes(5));
        }

    }
}