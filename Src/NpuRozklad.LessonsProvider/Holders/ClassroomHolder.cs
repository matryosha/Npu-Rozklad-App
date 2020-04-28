using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Infrastructure;
using NpuRozklad.LessonsProvider.Fetcher;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider.Holders
{
    public class ClassroomHolder : PeriodicOperationExecutor, IClassroomsHolder
    {
        private readonly INpuServerFetcher _fetcher;
        private readonly Dictionary<Faculty, List<Classroom>> _classroomsCache =
            new Dictionary<Faculty, List<Classroom>>();
        private readonly ConcurrentDictionary<Faculty, OneManyLock> _facultyLocks =
            new ConcurrentDictionary<Faculty, OneManyLock>();
        private readonly OneManyLock _cacheLock = new OneManyLock();
        // To options
        private int _cacheLifetimeInMinutes = 30;
        
        
        public ClassroomHolder(INpuServerFetcher fetcher)
        {
            _fetcher = fetcher;
            PeriodicCallIntervalInSeconds = _cacheLifetimeInMinutes * 60;
            PeriodicAction = ClearCache;
        }
        
        public async Task<ICollection<Classroom>> GetClassrooms(Faculty faculty)
        {

            var facultyLock = _facultyLocks.GetOrAdd(faculty, new OneManyLock());
            facultyLock.Enter(false);
            _cacheLock.Enter(false);

            if (!_classroomsCache.ContainsKey(faculty))
            {
                facultyLock.Leave();
                _cacheLock.Leave();
                facultyLock.Enter(true);
                _cacheLock.Enter(false);

                if (!_classroomsCache.ContainsKey(faculty))
                {
                    _cacheLock.Leave();
                    
                    var facultyClassroomsString = await _fetcher.FetchClassroom(faculty.TypeId);
                    var facultyClassrooms = RawStringParser.DeserializeClassrooms(facultyClassroomsString);
                    
                    _cacheLock.Enter(true);
                    _classroomsCache[faculty] = facultyClassrooms;
                }
            }
            
            var result = new List<Classroom>(_classroomsCache[faculty]);
            _cacheLock.Leave();
            facultyLock.Leave();

            return result;
        }

        private Task ClearCache()
        {
            _cacheLock.Enter(true);
            _classroomsCache.Clear();
            _cacheLock.Leave();
            return Task.CompletedTask;
        }
    }
}