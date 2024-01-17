using CommonBlazor.DynamicData;
using CommonBlazor.DynamicData.Abstractions;
using CommonBlazor.DynamicData.Filtering;
using CommonBlazor.Infrastructure;

namespace CommonBlazor.UI.Filtering
{
    public class FilteringService : IFilteringService
    {
        private ICustomFiltersProvider? _customFiltersProvider;

        private ICustomFiltersProvider CustomFiltersProvider { get => _customFiltersProvider ?? throw ThrowHelper.InjectIsNull(); set => _customFiltersProvider = value; }

        private ApplicationCache _applicationCache;

        private ApplicationCache ApplicationCache { get => _applicationCache ?? throw ThrowHelper.InjectIsNull(); set => _applicationCache = value; }

        public FilteringService(ICustomFiltersProvider customFiltersProvider, ApplicationCache applicationCache)
        {
            _customFiltersProvider = customFiltersProvider;
            _applicationCache = applicationCache;
        }

        public async Task<FilterInfo?> GetDefaultFilterAsync(string key, CancellationToken cancellationToken = default)
        {
            var result = await ApplicationCache.ContainsAsync<FilterInfo>(key);

            if(result == true)
                return await ApplicationCache.GetAsync<FilterInfo>(key, cancellationToken);

            var defaultFilter = await CustomFiltersProvider.GetDefaultFilterAsync(key, cancellationToken);

            await ApplicationCache.SetAsync<FilterInfo>(key, defaultFilter, cancellationToken);

            return defaultFilter;
        }
    }
}
