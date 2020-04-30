using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Infrastructure;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.LessonsProvider.Fetcher;

namespace NpuRozklad.LessonsProvider
{
    /// <summary>
    /// Uses <see cref="NpuRozklad.LessonsProvider.Fetcher.INpuServerFetcher"/> to fetch
    /// string with faculties and deserialize with <see cref="NpuRozklad.LessonsProvider.RawStringParser"/>
    /// to get collection of <see cref="NpuRozklad.Core.Entities.Faculty"/>. Caches values and auto updates.
    /// </summary>
    internal class FacultiesProvider : PeriodicOperationExecutor, IFacultiesProvider
    {
        private readonly INpuServerFetcher _fetcher;
        private readonly List<Faculty> _facultiesCache = new List<Faculty>();
        private readonly OneManyLock _cacheLock = new OneManyLock();
        // to options
        private int _cacheLifeTimeInMinutes = 30;

        public FacultiesProvider(INpuServerFetcher fetcher)
        {
            _fetcher = fetcher;

            PeriodicCallIntervalInSeconds = _cacheLifeTimeInMinutes * 60;
            PeriodicAction = UpdateCache;
        }
        
        /// <summary>
        /// Fetch and deserialize or get from its cache collection of <see cref="NpuRozklad.Core.Entities.Faculty"/>
        /// </summary>
        /// <returns>Collection of <see cref="NpuRozklad.Core.Entities.Faculty"/></returns>
        public async Task<ICollection<Faculty>> GetFaculties()
        {
            _cacheLock.Enter(false);

            if (!_facultiesCache.Any())
            {
                _cacheLock.Leave();
                _cacheLock.Enter(true);

                if (!_facultiesCache.Any())
                {
                    var faculties = await GetFacultiesInternal();
                    _facultiesCache.AddRange(faculties);
                }
            }

            var result = new List<Faculty>(_facultiesCache);
            _cacheLock.Leave();
            return result;
        }

        private async Task<List<Faculty>> GetFacultiesInternal()
        {
            var facultiesString = await _fetcher.FetchFaculties();
            var faculties = RawStringParser.DeserializeFaculties(facultiesString);
            return faculties;
        }

        private async Task UpdateCache()
        {
            var faculties = await GetFacultiesInternal();
            _cacheLock.Enter(true);
            _facultiesCache.Clear();
            _facultiesCache.AddRange(faculties);
            _cacheLock.Leave();
        }
    }
}