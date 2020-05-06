using System;
using System.Threading.Tasks;
using NpuRozklad.Core.Infrastructure;
using NpuRozklad.LessonsProvider.Fetcher;
using NpuRozklad.LessonsProvider.Holders.Interfaces;

namespace NpuRozklad.LessonsProvider.Holders
{
    internal class SettingsHolder : PeriodicOperationExecutor, ISettingsHolder
    {
        private readonly INpuServerFetcher _fetcher;
        private (DateTime date, bool isOddDay) _settingsCache;
        private bool _isSettingsFetched;
        private readonly OneManyLock _cacheLock = new OneManyLock();
        // to options
        private int _cacheLifeTimeInMinutes = 10;
        
        
        public SettingsHolder(INpuServerFetcher fetcher)
        {
            _fetcher = fetcher;
            PeriodicCallIntervalInSeconds = _cacheLifeTimeInMinutes * 60;
            PeriodicAction = UpdateCache;
        }
        
        public async Task<(DateTime date, bool IsOddDay)> GetSettings()
        {
            try
            {
                _cacheLock.Enter(false);
                if (!_isSettingsFetched)
                {
                    _cacheLock.Leave();
                    _cacheLock.Enter(true);

                    if (!_isSettingsFetched)
                    {
                        var settings = await GetSettingsInternal();

                        _settingsCache = settings;
                        _isSettingsFetched = true;
                    }
                }

                var result = _settingsCache;
                return result;
            }
            finally
            {
                _cacheLock.Leave();
            }
        }

        private async Task<(DateTime date, bool IsOddDay)> GetSettingsInternal()
        {
            var settingsString = await _fetcher.FetchSettings();
            var settings = RawStringParser.DeserializeSettings(settingsString);
            return settings;
        }

        private async Task UpdateCache()
        {
            (DateTime date, bool IsOddDay) updatedSettings;
            try
            {
                updatedSettings = await GetSettingsInternal();
            }
            catch (Exception)
            {
                return;
            }
            
            _cacheLock.Enter(true);
            _settingsCache = updatedSettings;
            _cacheLock.Leave();
        }
    }
}