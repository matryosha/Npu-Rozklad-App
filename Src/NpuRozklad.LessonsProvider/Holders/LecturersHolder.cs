using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Infrastructure;
using NpuRozklad.LessonsProvider.Fetcher;
using NpuRozklad.LessonsProvider.Holders.Interfaces;
using static NpuRozklad.LessonsProvider.Exceptions.ExceptionFilters;

namespace NpuRozklad.LessonsProvider.Holders
{
    internal class LecturersHolder : PeriodicOperationExecutor, ILecturersHolder
    {
        private readonly INpuServerFetcher _fetcher;
        private readonly Dictionary<Faculty, List<Lecturer>> _lecturersCache = new Dictionary<Faculty, List<Lecturer>>();
        private readonly Dictionary<Faculty, List<Lecturer>> _lecturersBackCache = new Dictionary<Faculty, List<Lecturer>>();
        private readonly ConcurrentDictionary<Faculty, OneManyLock> _facultyLocks =
            new ConcurrentDictionary<Faculty, OneManyLock>();
        private readonly OneManyLock _cacheLock = new OneManyLock();
        //to options
        private int _cacheLifeTimeInMinutes = 30;
        
        public LecturersHolder(INpuServerFetcher fetcher)
        {
            _fetcher = fetcher;
            PeriodicCallIntervalInSeconds = _cacheLifeTimeInMinutes * 60;
            PeriodicAction = ClearCache;
        }

        public async Task<ICollection<Lecturer>> GetLecturers(Faculty faculty)
        {
            var facultyLock = _facultyLocks.GetOrAdd(faculty, new OneManyLock());
            try
            {
                facultyLock.Enter(false);
                _cacheLock.Enter(false);

                if(!_lecturersCache.ContainsKey(faculty))
                {
                    facultyLock.Leave();
                    _cacheLock.Leave();
                    facultyLock.Enter(true);
                    _cacheLock.Enter(false);
                
                    if (!_lecturersCache.ContainsKey(faculty))
                    {
                        _cacheLock.Leave();

                        var lecturers = await GetLecturersInternal(faculty);
                    
                        _cacheLock.Enter(true);
                        _lecturersCache.Add(faculty, lecturers);
                        _lecturersBackCache[faculty] = lecturers;
                    }
                }

                var result = new List<Lecturer>(_lecturersCache[faculty]);
                return result;
            }
            finally
            {
                _cacheLock.Leave();
                facultyLock.Leave();
            }
        }

        private async Task<List<Lecturer>> GetLecturersInternal(Faculty faculty)
        {
            try
            {
                var lecturersString = await _fetcher.FetchLecturers(faculty.TypeId);
                var lecturers = RawStringParser.DeserializeLecturers(lecturersString);
                return lecturers;
            }
            catch (Exception e) when (DeserializationOrFetchException(e))
            {
                if (_lecturersBackCache.TryGetValue(faculty, out var backCacheResult))
                {
                    return backCacheResult;
                }

                throw;
            }
        }

        private Task ClearCache()
        {
            _cacheLock.Enter(true);
            _lecturersCache.Clear();
            _cacheLock.Leave();
            
            return Task.CompletedTask;
        }
    }
}