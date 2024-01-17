using CommonBlazor.DynamicData.Models;
using CommonBlazor.Infrastructure;

namespace CommonBlazor.DynamicData
{
    public class DynamicEntityContextInitializer
    {
        private readonly IDynamicDataProvider _dynamicDataProvider;
        private ApplicationCache _applicationCache;
        private Dictionary<string, Task<ConfigurationModel>> _tasks = new Dictionary<string, Task<ConfigurationModel>>();

        public DynamicEntityContextInitializer(IDynamicDataProvider dynamicDataProvider, ApplicationCache applicationCache)
        {
            _dynamicDataProvider = dynamicDataProvider;
            _applicationCache = applicationCache;
        }

        public async Task InitializeContextAsync(DynamicEntityContext context, CancellationToken cancellationToken = default)
        {
            var storageKey = context.Key;

            var value = await _applicationCache.GetAsync<DynamicEntityContext>(storageKey);

            if (value != null)
            {
                context.Properties = value.Properties;
                context.EntityName = value.EntityName;
            }
            else if (_tasks.ContainsKey(storageKey))
            {
                var configuration = await _tasks[storageKey];

                context.Properties = configuration.Columns;
                context.EntityName = configuration.EntityName;
            }
            else
            {
                var task = _dynamicDataProvider.GetConfigurationModelAsync(context.Key, cancellationToken);

                _tasks.Add(storageKey, task);

                var configuration = await task;

                _tasks.Remove(storageKey);

                context.Properties = configuration.Columns;
                context.EntityName = configuration.EntityName;

                await _applicationCache.SetAsync<DynamicEntityContext>(storageKey, context, cancellationToken);
            }

            context.IsInitialized = true;
        }
    }
}
