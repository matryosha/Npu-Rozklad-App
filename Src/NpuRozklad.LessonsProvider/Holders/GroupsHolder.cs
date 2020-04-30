using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.Core.Entities;
using NpuRozklad.Core.Infrastructure;
using NpuRozklad.Core.Interfaces;
using NpuRozklad.LessonsProvider.Fetcher;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider.Holders
{
    public class GroupsHolder : PeriodicOperationExecutor, IGroupsHolder
    {
        private readonly INpuServerFetcher _fetcher;
        private readonly IFacultiesProvider _facultiesProvider;

        private readonly List<Group> _groupsCache = new List<Group>();
        private readonly OneManyLock _cacheLock = new OneManyLock();
        // to options
        private int _cacheLifeTimeInMinutes = 30;
        
        public GroupsHolder(INpuServerFetcher fetcher, IFacultiesProvider facultiesProvider)
        {
            _fetcher = fetcher;
            _facultiesProvider = facultiesProvider;
            PeriodicCallIntervalInSeconds = _cacheLifeTimeInMinutes * 60;
            PeriodicAction = UpdateCache;
        }

        public async Task<ICollection<Group>> GetFacultiesGroups()
        {
            // Todo holder for every faculty
            
            _cacheLock.Enter(false);
            if (!_groupsCache.Any())
            {
                _cacheLock.Leave();
                _cacheLock.Enter(true);
                if (!_groupsCache.Any())
                {
                    var facultiesGroups = await GetFacultiesGroupsInternal();

                    _groupsCache.AddRange(facultiesGroups);
                }
            }
            var result = new List<Group>(_groupsCache);
            _cacheLock.Leave();
            return result;
        }

        private async Task<List<Group>> GetFacultiesGroupsInternal()
        {
            var faculties = await _facultiesProvider.GetFaculties();
            var facultiesGroups = new List<Group>();
            foreach (var faculty in faculties)
            {
                var groupsString = await _fetcher.FetchGroups(faculty.TypeId);
                var groups = RawStringParser.DeserializeGroups(groupsString, faculty);

                facultiesGroups.AddRange(groups);
            }

            return facultiesGroups;
        }

        private async Task UpdateCache()
        {
            var facultiesGroups = await GetFacultiesGroupsInternal();
            _cacheLock.Enter(true);
            _groupsCache.Clear();
            _groupsCache.AddRange(facultiesGroups);
            _cacheLock.Leave();
        }
    }
}