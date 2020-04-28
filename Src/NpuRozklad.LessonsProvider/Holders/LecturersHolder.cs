using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Infrastructure;
using NpuRozklad.LessonsProvider.Fetcher;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider.Holders
{
    public class LecturersHolder : PeriodicOperationExecutor, ILecturersHolder
    {
        private readonly INpuServerFetcher _fetcher;
        private readonly Dictionary<Faculty, List<Lecturer>> _lecturersCache = new Dictionary<Faculty, List<Lecturer>>();
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
                }
            }

            var result = new List<Lecturer>(_lecturersCache[faculty]);
            _cacheLock.Leave();
            facultyLock.Leave();
            
            return result;
        }

        private async Task<List<Lecturer>> GetLecturersInternal(Faculty faculty)
        {
            var lecturersString = await _fetcher.FetchLecturers(faculty.TypeId);
            var lecturers = RawStringParser.DeserializeLecturers(lecturersString);
            return lecturers;
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