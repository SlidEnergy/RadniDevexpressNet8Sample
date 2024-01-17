using System.Collections;
using System.Dynamic;
using DevExpress.Blazor;
using DevExpress.Data.Filtering;
using Common.DataAccess.Filtering;
using Common.Windows.Utils;
using CommonBlazor.DynamicData;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.Infrastructure;
using CommonBlazor.Extensions;
using DevExpress.XtraExport.Xls;
using DevExtreme.AspNet.Data;

namespace CommonBlazor.UI.List
{
    public class LoadOnDemandHeaderFilterDynamicEntityDataSource<T> : GridCustomDataSourceBase where T : class
    {
        private readonly IDynamicDataProvider _dynamicDataProvider;
        private readonly DynamicEntityContext _entityContext;
        private IPropertyNameResolveProvider _propertyNameResolveProvider;
        //private ExceptionHandler _exceptionHandler;
        private readonly GenericColumnSettings _property;

        public override event Action? AllDataRefreshed;

        private IPropertyNameResolveProvider PropertyNameResolveProvider { get => _propertyNameResolveProvider ?? throw ThrowHelper.InjectIsNull(); set => _propertyNameResolveProvider = value; }

        public LoadOnDemandHeaderFilterDynamicEntityDataSource(string fieldName, DynamicEntityContext dynamicEntityContext, EntityGridControllerBase<T> controller)
        {
            var property = dynamicEntityContext.VisibleProperties.FirstOrDefault(x =>
                controller.ConvertPropertyName(x.FullPropertyName) == fieldName);

            if (property == null)
                throw new Exception("Column property name not found in the column collection.");

            _property = property;

            _dynamicDataProvider = ServiceResolver.Resolve<IDynamicDataProvider>();
            _entityContext = dynamicEntityContext;

            _propertyNameResolveProvider = ServiceResolver.Resolve<IPropertyNameResolveProvider>();
            //_exceptionHandler = ServiceResolver.Resolve<ExceptionHandler>();
        }

        public override async Task<int> GetItemCountAsync(GridCustomDataSourceCountOptions options, CancellationToken cancellationToken)
        {
            DataIsLoaded = false;

            try
            {
                var filter = ConstructFilter(options.FilterCriteria);
                var collectionFilters = GetCollectionFilters();
                var quickFilters = GetQuickFilters();

                var result = await _dynamicDataProvider.GetItemCountAsync(_entityContext.Key,
                    new List<GenericColumnSettings>()
                    {
                        _property
                    },
                    filter?.ToString(),
                    collectionFilters,
                    quickFilters?.ToString(),
                    true,
                    cancellationToken);

                AllDataRefreshed?.Invoke();

                return result;
            }
            catch (Exception ex)
            {
                //await _exceptionHandler.HandleAsync(ex, true);
                throw;
            }
        }

        public override async Task<IList> GetItemsAsync(GridCustomDataSourceItemsOptions options, CancellationToken cancellationToken)
        {
            try
            {
                var filter = ConstructFilter(options.FilterCriteria);
                var collectionFilters = GetCollectionFilters();
                var quickFilters = GetQuickFilters();

                var sort = new List<DynamicData.Models.ListSortModel>
                {
                    new ()
                    {
                        Ascending = true,
                        ColumnName = _property.FullPropertyName
                    }
                };

                var result = await _dynamicDataProvider.GetListAsync(_entityContext.Key,
                    new List<GenericColumnSettings>()
                    {
                        _property
                    },
                    filter?.ToString(),
                    collectionFilters,
                    quickFilters?.ToString(),
                    sort,
                    new List<string>() { _property.FullPropertyName },
                    options.StartIndex,
                    options.Count,
                    cancellationToken);

                DataIsLoaded = true;

                return (IList)result;
            }
            catch (Exception ex)
            {
               // await _exceptionHandler.HandleAsync(ex, true);
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
    }
}
