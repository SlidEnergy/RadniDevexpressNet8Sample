using System.Dynamic;
using CommonBlazor.DynamicData.Models;

namespace CommonBlazor.DynamicData
{
    public interface IDynamicDataProvider
    {
        Task<IEnumerable<GenericColumnSettings>> GetAvailableColumnsAsync(string key, string? path, CancellationToken cancellation = default);
        Task<IEnumerable<GenericCollectionSettings>> GetAvailableCollectionsAsync(string key, string? path, CancellationToken cancellation = default);
        Task<ConfigurationModel> GetConfigurationModelAsync(string key, CancellationToken cancellationToken = default);
        Task<IList<ExpandoObject>> GetListAsync(string key, IEnumerable<GenericColumnSettings> properties, string? filter, Dictionary<string, string>? collectionFilters, string? quickFilter,
            IEnumerable<ListSortModel>? sortByColumns, IEnumerable<string>? groupByColumns, int pageNumber, int pageSize,
            CancellationToken cancellationToken = default);

        Task<IList<ExpandoObject>> GetAllAsync(string key, IEnumerable<GenericColumnSettings> properties, IEnumerable<ListSortModel>? sortByColumns, CancellationToken cancellationToken = default);

        Task<int> GetItemCountAsync(string key, IEnumerable<GenericColumnSettings> properties, string? filter,
            Dictionary<string, string> collectionFilters, string? quickFilter, bool unique,
            CancellationToken cancellationToken = default);
        
        Task<IEnumerable<GenericCollectionSettings>> SaveColumns(string key, IEnumerable<GenericColumnSettings> properties, CancellationToken cancellationToken = default);
    }
}