using System.Collections;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.Extensions;
using CommonBlazor.Infrastructure;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Reflection.Metadata;
using Common.DataAccess.Filtering;

namespace CommonBlazor.DynamicData
{
    public class DynamicEntityService<TKeyField> : IDynamicEntityService
    {
        private readonly IDynamicDataProvider _dynamicDataProvider;
        private readonly DynamicEntityContext _entityContext;

        private Dictionary<TKeyField, ExpandoObject>? _entitiesMap = null;
        private Dictionary<TKeyField, int>? _entitiesOrderMap = null;
        private List<ExpandoObject>? _entitiesList = null;
        private int _count = 0;

        private IEnumerable<GenericColumnSettings>? _lastProperties;
        private string? _lastFilter;
        private Dictionary<string, string> _lastCollectionFilters;
        private string? _lastQuickFilters;
        IEnumerable<ListSortModel>? _lastSortByColumns;
        private IEnumerable<string>? _lastGroupByColumns;

        private object[]? _lastGetCountItemsParameters;

        public Type KeyFieldType => typeof(TKeyField);

        public DynamicEntityService(DynamicEntityContext entityContext)
        {
            _entityContext = entityContext;
            _dynamicDataProvider = ServiceResolver.Resolve<IDynamicDataProvider>(); ;
        }

        public async Task<IEnumerable<ExpandoObject>> GetListAsync(bool refreshFromServer, bool groupInfoRequest, IEnumerable<GenericColumnSettings> properties, string? filter,
            Dictionary<string, string> collectionFilters, string? quickFilters, IEnumerable<ListSortModel>? sortByColumns,
            IEnumerable<string>? groupByColumns, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var lastParameters = new object[]
                {
                    _lastProperties, _lastFilter, _lastCollectionFilters, _lastQuickFilters, _lastSortByColumns,
                    _lastGroupByColumns
                };

            var initialLoad = refreshFromServer || _entitiesList == null || _entitiesList.Count != _count ||
                              !IsParametersSame(lastParameters, properties, filter, collectionFilters, quickFilters, sortByColumns, groupByColumns);

            // initial load list
            if (initialLoad)
            {
                if (groupInfoRequest)
                    _count = 0;

                _entitiesList = null;

                return await GetListFromServer(properties, filter, collectionFilters, quickFilters, sortByColumns,
                    groupByColumns, pageNumber, pageSize, cancellationToken);
            }

            var pagingEnabled = pageNumber + pageSize > 0;

            // load full list from cache
            if (!pagingEnabled)
            {
                return _entitiesList.ToList();
            }

            var (emptyStart, emptyCount) = GetEmptyItems(pageNumber, pageSize);

            if (emptyCount > 0)
            {
                await GetListFromServer(properties, filter, collectionFilters, quickFilters, sortByColumns,
                    groupByColumns, emptyStart, emptyCount,
                    cancellationToken);
            }

            return _entitiesList.GetRange(pageNumber, pageNumber + pageSize > _count ? _count - pageNumber : pageSize);
        }

        private (int, int) GetEmptyItems(int pageNumber, int pageSize)
        {
            var end = Math.Min(_count, pageNumber + pageSize);
            int start = end;
            int count = 0;
            bool startCount = false;

            if (_entitiesList == null)
                return (start, count);

            for (var i = pageNumber; i < end; i++)
            {
                var item = _entitiesList[i];

                if (!startCount && item == null)
                {
                    start = i;
                    startCount = true;
                }

                if (!startCount)
                    continue;

                if (item != null)
                    break;

                count++;
            }

            return new(start, count);
        }

        private async Task<IList<ExpandoObject>> GetListFromServer(IEnumerable<GenericColumnSettings> properties, string? filter,
            Dictionary<string, string> collectionFilters, string? quickFilters, IEnumerable<ListSortModel>? sortByColumns,
            IEnumerable<string>? groupByColumns, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var list = await _dynamicDataProvider.GetListAsync(_entityContext.Key, properties, filter,
                collectionFilters, quickFilters, sortByColumns,
                groupByColumns, pageNumber, pageSize, cancellationToken);

            var pagingEnabled = pageNumber + pageSize > 0;

            if (!pagingEnabled)
                _count = list.Count;

            MergeList(list, pageNumber);

            _lastProperties = properties;
            _lastFilter = filter;
            _lastCollectionFilters = collectionFilters;
            _lastQuickFilters = quickFilters;
            _lastSortByColumns = sortByColumns;
            _lastGroupByColumns = groupByColumns;

            return list;
        }

        private void MergeList(IList<ExpandoObject> list, int offset)
        {
            if (_entitiesList == null)
            {
                _entitiesList = Enumerable.Range(0, Math.Max(_count, list.Count)).Select<int, ExpandoObject>(_ => null).ToList();
                _entitiesMap = new Dictionary<TKeyField, ExpandoObject>();
                _entitiesOrderMap = new Dictionary<TKeyField, int>();
            }

            if (_entitiesList.Count < offset + list.Count)
                throw new Exception("Out of range");

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];

                if (item == null)
                    continue;

                var keyField =
                    (TKeyField)item.GetValue(_entityContext.KeyField
                        .ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString());

                _entitiesList[offset + i] = item;
                _entitiesMap.TryAdd(keyField, item);
                _entitiesOrderMap.TryAdd(keyField, offset + i);
            }
        }

        private bool IsParametersSame(object[] savedParameters, params object[] parameters)
        {
            if (savedParameters == null)
                return false;

            if (parameters.Length != savedParameters.Length)
                return false;

            for (var p = 0; p < savedParameters.Length; p++)
            {
                var last = savedParameters[p];
                var param = parameters[p];

                var lastType = last?.GetType();
                var paramType = param?.GetType();

                if (lastType != paramType || (last == null && param != null) || (param == null && last != null))
                    return false;

                if (last == null && param == null)
                    continue;

                if (typeof(IEnumerable).IsAssignableFrom(paramType))
                {
                    var lastEnumerator = ((IEnumerable)last).GetEnumerator();
                    var paramEnumerator = ((IEnumerable)param).GetEnumerator();

                    var lastHasNext = lastEnumerator.MoveNext();
                    var paramHasNext = paramEnumerator.MoveNext();

                    while (lastHasNext && paramHasNext)
                    {
                        var lastItem = lastEnumerator.Current;
                        var paramItem = paramEnumerator.Current;

                        var lastItemType = lastItem?.GetType();
                        var paramItemType = paramItem?.GetType();

                        if (lastItemType != paramItemType || (lastItem == null && paramItem != null) || (paramItem == null && lastItem != null))
                            return false;

                        if (lastItem == null && paramItem == null)
                            continue;

                        // TODO: Test compare Dictionary
                        if (lastItemType == typeof(KeyValuePair<string, string>))
                        {
                            if (((KeyValuePair<string, string>)lastItem).Key.Equals(
                                    ((KeyValuePair<string, string>)paramItem).Key) ||
                                ((KeyValuePair<string, string>)lastItem).Value.Equals(
                                    ((KeyValuePair<string, string>)paramItem).Value))
                                return false;
                        }
                        else if (!lastItem.Equals(paramItem))
                            return false;

                        lastHasNext = lastEnumerator.MoveNext();
                        paramHasNext = paramEnumerator.MoveNext();
                    }

                    // Length of enumerables are different
                    if (lastHasNext || paramHasNext)
                        return false;
                }
                else
                {
                    if (!param.Equals(last))
                        return false;
                }
            }

            return true;
        }

        public async Task<int> GetItemCountAsync(bool refreshFromServer, IEnumerable<GenericColumnSettings> properties,
            string? filter,
            Dictionary<string, string> collectionFilters, string? quickFilter, bool unique,
            CancellationToken cancellationToken = default)
        {
            if (refreshFromServer || _entitiesList == null || _entitiesList.Count != _count || !IsParametersSame(_lastGetCountItemsParameters, properties, filter, collectionFilters, quickFilter, unique))
            {
                return await GetItemCountFromServerAsync(properties, filter, collectionFilters, quickFilter, unique, cancellationToken);
            }

            return _count;
        }

        private async Task<int> GetItemCountFromServerAsync(IEnumerable<GenericColumnSettings> properties, string? filter,
            Dictionary<string, string> collectionFilters, string? quickFilter, bool unique,
            CancellationToken cancellationToken = default)
        {
            var count = await _dynamicDataProvider.GetItemCountAsync(_entityContext.Key, properties, filter, collectionFilters, quickFilter, unique, cancellationToken);

            _lastGetCountItemsParameters = new object[] { properties, filter, collectionFilters, quickFilter, unique };
            _count = count;

            return count;
        }

        private async Task<ExpandoObject> GetEntityFromServerAsync(TKeyField entityId, CancellationToken cancellationToken = default)
        {
            var filter = _lastFilter == null ? null : FilterCriteria.Parse(_lastFilter);
            var idFilter = CriteriaFactory.Equal(_entityContext.KeyField, entityId);

            if (filter == null)
                filter = idFilter;
            else
                filter = CriteriaFactory.And(filter, idFilter);

            var list = await _dynamicDataProvider.GetListAsync(_entityContext.Key, _lastProperties, filter.ToString(),
                _lastCollectionFilters, _lastQuickFilters, _lastSortByColumns, _lastGroupByColumns, 0, 0, cancellationToken);

            if (list == null || list.Count == 0)
                return null;

            return list[0];
        }

        public async Task UpdateEntityAsync(object entityId, CancellationToken cancellationToken = default)
        {
            if (_entitiesMap == null || _entitiesOrderMap == null || _entitiesList == null || entityId == null)
                return;

            if (_entitiesMap.ContainsKey((TKeyField)entityId))
            {
                var entity = await GetEntityFromServerAsync((TKeyField)entityId, cancellationToken);

                _entitiesMap[(TKeyField)entityId] = entity;
                var order = _entitiesOrderMap[(TKeyField)entityId];

                if (order >= _entitiesList.Count)
                    throw new Exception("Order out of range");

                _entitiesList[order] = entity;
            }
        }

        public async Task AddEntityAsync(object entityId, CancellationToken cancellationToken = default)
        {
            if (_entitiesMap == null || _entitiesOrderMap == null || _entitiesList == null || entityId == null)
                return;

            if (!_entitiesMap.ContainsKey((TKeyField)entityId))
            {
                var entity = await GetEntityFromServerAsync((TKeyField)entityId, cancellationToken);

                _entitiesMap.Add((TKeyField)entityId, entity);
                _entitiesOrderMap.Add((TKeyField)entityId, _count);

                _entitiesList.Add(entity);
                _count++;
            }
        }

        public void DeleteEntity(object entityId)
        {
            if (_entitiesMap == null || _entitiesOrderMap == null || _entitiesList == null || entityId == null)
                return;

            if (_entitiesMap.ContainsKey((TKeyField)entityId))
            {
                _entitiesMap.Remove((TKeyField)entityId);
                var order = _entitiesOrderMap[(TKeyField)entityId];

                if (order >= _entitiesList.Count)
                    throw new Exception("Order out of range");

                _entitiesList.RemoveAt(order);

                // Refresh orders for all items
                var keyFieldName = _entityContext.KeyField.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString();

                for (var i = 0; i < _entitiesList.Count; i++)
                {
                    var item = _entitiesList[i];

                    if (item != null)
                    {
                        _entitiesOrderMap.TryAdd((TKeyField)item.GetValue(keyFieldName), i);
                    }
                }

                _count--;
            }
        }
    }
}
