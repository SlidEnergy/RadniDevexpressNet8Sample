using Common.DataAccess.Filtering;
using Common.Windows.Utils;
using CommonBlazor.Infrastructure;
using CommonBlazor.Infrastructure.Dispatcher;
using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using System.Collections;

namespace CommonBlazor.UI.List
{
    public class LoadOnDemandEntityDataSource<T> : GridCustomDataSourceBase where T : class
    {
        private readonly EntityContext _entityContext;
        //private ExceptionHandler _exceptionHandler;
        private IDispatcher _dispatcher;

        public override event Action? AllDataRefreshed;

        public LoadOnDemandEntityDataSource(EntityContext entityContext)
        {
            //_exceptionHandler = ServiceResolver.Resolve<ExceptionHandler>();
            _dispatcher = ServiceResolver.Resolve<IDispatcher>();
            _entityContext = entityContext;
        }

        public async override Task<int> GetItemCountAsync(GridCustomDataSourceCountOptions options, CancellationToken cancellationToken)
        {
            DataIsLoaded = false;

            try
            {
                var gridFilter = ConstructFilter(options.FilterCriteria);

                var collectionFilters = GetCollectionFilters();

                var quickFilters = GetQuickFilters();

                var request = (EntityQueryRequest?)Activator.CreateInstance(_entityContext.GetCountRequest);


                if(request == null)
                    throw new Exception("Couldn't create instance of " + _entityContext.GetCountRequest);

                request.FilterText = gridFilter?.ToString();
                request.CollectionFilters = collectionFilters;
                request.QuickFilter = quickFilters?.ToString();

                var count = await _dispatcher.Send(request);

                AllDataRefreshed?.Invoke();

                return (int)count;
            }
            catch (Exception ex)
            {
                //await _exceptionHandler.HandleAsync(ex, true);
                throw;
            }
        }

        public async override Task<IList> GetItemsAsync(GridCustomDataSourceItemsOptions options, CancellationToken cancellationToken)
        {
            try
            {
                var sort = ConvertSort(options.SortInfo);
                var gridFilter = ConstructFilter(options.FilterCriteria);

                var collectionFilters = GetCollectionFilters();

                var quickFilters = GetQuickFilters();

                var pageSize = options.Count;

                var request = (EntityQueryRequest?)Activator.CreateInstance(_entityContext.QueryRequest);

                if (request == null)
                    throw new Exception("Couldn't create instance of " + _entityContext.QueryRequest);

                request.SkipRows = options.StartIndex;
                request.TakeRows = pageSize;
                request.FilterText = gridFilter?.ToString();
                request.CollectionFilters = collectionFilters;
                request.QuickFilter = quickFilters?.ToString();
                request.SortByColumns = sort;
                request.GroupByColumns = null;

                var result = await _dispatcher.Send(request);

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

            if (Filter != null)
            {
                if (filter == null)
                    filter = Filter;
                else
                    filter = CriteriaFactory.And(filter, Filter);
            }

            return filter;
        }

        private ListSortModel[]? ConvertSort(IReadOnlyList<GridCustomDataSourceSortInfo> sortInfo)
        {
            if (sortInfo == null || sortInfo.Count < 1)
                return null;

            return sortInfo
                .Select(i => new ListSortModel
                {
                    ColumnName = i.FieldName,
                    Ascending = !i.DescendingSortOrder
                })
                .ToArray();
        }
    }
}
