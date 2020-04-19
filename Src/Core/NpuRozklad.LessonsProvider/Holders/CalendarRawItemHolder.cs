using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NpuRozklad.LessonsProvider.Entities;
using NpuRozklad.LessonsProvider.Fetcher;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider.Holders
{
    public class CalendarRawItemHolder : PeriodicOperationExecutor, ICalendarRawItemHolder
    {
        private readonly INpuServerFetcher _fetcher;
        private List<CalendarRawItem> _calendarRawItemsCache = new List<CalendarRawItem>();
        private readonly OneManyLock _cacheLock = new OneManyLock();
        // To options
        private int _cacheLifeTimeMinutes = 10;

        public CalendarRawItemHolder(INpuServerFetcher fetcher)
        {
            _fetcher = fetcher;
            PeriodicCallIntervalInSeconds = _cacheLifeTimeMinutes * 60;
            PeriodicAction = UpdateCache;
        }

        public async Task<ICollection<CalendarRawItem>> GetCalendarItems()
        {
            ICollection<CalendarRawItem> result;
            _cacheLock.Enter(false);
            
            if (!_calendarRawItemsCache.Any())
            {
                _cacheLock.Leave();
                _cacheLock.Enter(true);
                if (!_calendarRawItemsCache.Any())
                {
                    var calendarItems = await GetCalendarRawItemsInternal();
                    _calendarRawItemsCache = calendarItems;
                }
            }

            result = new List<CalendarRawItem>(_calendarRawItemsCache);
            _cacheLock.Leave();
            return result;
        }

        private async Task<List<CalendarRawItem>> GetCalendarRawItemsInternal()
        {
            var calendarRawItemsString = await _fetcher.FetchCalendar();
            var calendarItems = RawStringParser.DeserializeCalendar(calendarRawItemsString);
            return calendarItems;
        }

        private async Task UpdateCache()
        {
            // what if exception?
            var calendarItems = await GetCalendarRawItemsInternal();
            
            _cacheLock.Enter(true);
            _calendarRawItemsCache = calendarItems;
            _cacheLock.Leave();
        }
    }
}