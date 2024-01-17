using System.Dynamic;
using System.Text.Json;
using CommonBlazor.DynamicData.Models;

namespace CommonBlazor.DynamicData;

public interface IDynamicEntityService
{
    Type KeyFieldType { get; }

    Task<IEnumerable<ExpandoObject>> GetListAsync(bool refreshFromServer, bool groupInfoRequest, IEnumerable<GenericColumnSettings> properties, string? filter,
        Dictionary<string, string> collectionFilters, string? quickFilters, IEnumerable<ListSortModel>? sortByColumns,
        IEnumerable<string>? groupByColumns, int pageNumber, int pageSize, CancellationToken cancellationToken);
    
    Task<int> GetItemCountAsync(bool refreshFromServer, IEnumerable<GenericColumnSettings> properties, string? filter,
        Dictionary<string, string> collectionFilters, string? quickFilter, bool unique,
        CancellationToken cancellationToken = default);

    Task UpdateEntityAsync(object entityId, CancellationToken cancellationToken = default);
    Task AddEntityAsync(object entityId, CancellationToken cancellationToken = default);
    
    void DeleteEntity(object entityId);
}