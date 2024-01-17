using Common.DataAccess.Filtering;
using Common.Windows.Utils;
using CommonBlazor.DynamicData;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.Extensions;
using CommonBlazor.Infrastructure;
using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using System.Collections;

namespace CommonBlazor.UI.List
{
    public class LoadOnDemandDynamicEntityDataSource<T> : GridCustomDataSourceBase where T : class
    {
        private readonly DynamicEntityContext _entityContext;
        private readonly EntityGridControllerBase<T> _controller;
        private IPropertyNameResolveProvider _propertyNameResolveProvider;
       // private ExceptionHandler _exceptionHandler;
        private IDynamicEntityService _entityService;

        public override event Action? AllDataRefreshed;
        public bool RefreshFromServer { get; set; }

        private IPropertyNameResolveProvider PropertyNameResolveProvider { get => _propertyNameResolveProvider ?? throw ThrowHelper.InjectIsNull(); set => _propertyNameResolveProvider = value; }

        public LoadOnDemandDynamicEntityDataSource(DynamicEntityContext dynamicEntityContext, EntityGridControllerBase<T> controller)
        {
            var entitiesService = ServiceResolver.Resolve<IDynamicEntities>();
            _entityService = entitiesService.GetOrCreateEntityService(dynamicEntityContext);

            _entityContext = dynamicEntityContext;
            _controller = controller;

            _propertyNameResolveProvider = ServiceResolver.Resolve<IPropertyNameResolveProvider>();
           // _exceptionHandler = ServiceResolver.Resolve<ExceptionHandler>();
        }

        public override async Task<int> GetItemCountAsync(GridCustomDataSourceCountOptions options, CancellationToken cancellationToken)
        {
            DataIsLoaded = false;

            try
            {
                var gridFilter = ConstructFilter(options.FilterCriteria);

                var collectionFilters = GetCollectionFilters();

                var quickFilters = GetQuickFilters();

                var count = await _entityService.GetItemCountAsync(RefreshFromServer, _entityContext.Properties.Take(1), gridFilter?.ToString(),
                    collectionFilters, quickFilters?.ToString(), false, cancellationToken);

                AllDataRefreshed?.Invoke();

                return count;
            }
            catch (Exception ex)
            {
              //  await _exceptionHandler.HandleAsync(ex, true);
                throw;
            }
        }

        public override async Task<IList> GetItemsAsync(GridCustomDataSourceItemsOptions options, CancellationToken cancellationToken)
        {
            try
            {
                var groupInfoRequest = options.ParentGroupInfo == null && options.SortInfo == null && options.FilterCriteria is null && options.Count == 1 && options.StartIndex == 0;

                var sort = ConstructSort(options.SortInfo);
                var filter = ConstructFilter(options.FilterCriteria);

                var collectionFilters = GetCollectionFilters();

                var quickFilters = GetQuickFilters();

                var pageSize = options.Count;

                var result = await _entityService.GetListAsync(RefreshFromServer, groupInfoRequest, _entityContext.Properties,
                    filter?.ToString(), collectionFilters, quickFilters?.ToString(), sort, null, options.StartIndex, pageSize, cancellationToken);

                if(!groupInfoRequest)
                    RefreshFromServer = false;

                DataIsLoaded = true;

                return (IList)result;
            }
            catch (Exception ex)
            {
                //await _exceptionHandler.HandleAsync(ex, true);
                throw;
            }
        }

        private Dictionary<string, string> GetCollectionFilters()
        {
            return CollectionFilters == null ? null : CollectionFilters.ToDictionary(x => x.Key, x => x.Value.ToString());
        }

        private FilterCriteria? GetQuickFilters()
        {
            if (QuickFilters == null || QuickFilters.Count() == 0)
                return null;

            if (QuickFilters.Count() == 1)
                return QuickFilters.First();

            var filter = QuickFilters.First();

            foreach (var criteria in QuickFilters.Skip(1))
            {
                filter = CriteriaFactory.And(filter, criteria);
            }

            return filter;
        }

        private FilterCriteria? ConstructFilter(CriteriaOperator filterCriteria)
        {
            var converter = new DXCriteriaOperatorConverter();
            FilterCriteria? filter = converter.Convert(filterCriteria);

            var propertyNameResolver = PropertyNameResolveProvider.GetUpperCasePropertyNameResolver(_entityContext.Properties);

            propertyNameResolver.ResolvePropertyNames(filter);

            if (Filter != null)
            {
                if (filter == null)
                    filter = Filter;
                else
                    filter = CriteriaFactory.And(filter, Filter);
            }

            return filter;
        }

        private DynamicData.Models.ListSortModel[]? ConstructSort(IReadOnlyList<GridCustomDataSourceSortInfo> sortInfo)
        {
            if ((sortInfo == null || sortInfo.Count < 1) && (SortBy == null || SortBy.Count() < 1))
                return null;

            var sortList = new List<CommonBlazor.DynamicData.Models.ListSortModel>();

            if (sortInfo != null)
            {
                var propertyNameResolver = PropertyNameResolveProvider.GetUpperCasePropertyNameResolver(_entityContext.Properties);

                sortList.AddRange(
                    sortInfo.Select(i => new CommonBlazor.DynamicData.Models.ListSortModel
                    {
                        ColumnName = propertyNameResolver.ResolvePropertyName(i.FieldName),
                        Ascending = !i.DescendingSortOrder
                    })
                );
            }
            
            if (SortBy != null && SortBy.Count() > 0)
            {
                sortList.AddRange(
                    SortBy.Select(x => new DynamicData.Models.ListSortModel()
                    {
                        ColumnName = x.FieldName,
                        Ascending = x.Direction == Common.DataAccess.Sorting.SortByDirection.Ascending
                    })
                );
            }

            return sortList.ToArray();
        }

        public override async Task<object[]> GetUniqueValuesAsync(GridCustomDataSourceUniqueValuesOptions options, CancellationToken cancellationToken)
        {
            try
            {
                var property = _entityContext.VisibleProperties.FirstOrDefault(x =>
                _controller.ConvertPropertyName(x.FullPropertyName) == options.FieldName);

                if (property == null)
                    throw new Exception("Column property name not found in the column collection.");

                var columnName = property.FullPropertyName;
                var filter = ConstructFilter(options.FilterCriteria);
                var collectionFilters = GetCollectionFilters();
                var quickFilters = GetQuickFilters();

                var sort = new List<DynamicData.Models.ListSortModel>
                {
                    new ()
                    {
                        Ascending = true,
                        ColumnName = columnName
                    }
                };

                var list = await _entityService.GetListAsync(
                    true,
                    false,
                    new List<GenericColumnSettings>()
                    {
                        property
                    },
                    filter?.ToString(),
                    collectionFilters,
                    quickFilters?.ToString(),
                    sort,
                    new List<string>() { columnName },
                    0,
                    0,
                    cancellationToken);

                // TODO: add method returns list to dynamicEntityService
                var uniqueList = list.Select(x =>
                {
                    x.TryGetValue(columnName.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString(), out var value);
                    return value;
                }).Distinct().ToArray();

                return uniqueList;
            }
            catch (Exception ex)
            {
               // await _exceptionHandler.HandleAsync(ex, true);
                throw;
            }
        }
    }
}
