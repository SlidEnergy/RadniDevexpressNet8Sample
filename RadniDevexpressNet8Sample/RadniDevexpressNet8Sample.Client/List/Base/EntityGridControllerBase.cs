using System.Dynamic;
using Common.DataAccess.Filtering;
using Common.DataAccess.Sorting;
using CommonBlazor.DynamicData;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.Extensions;
using CommonBlazor.Infrastructure;
using CommonBlazor.UI.Configuration;
using CommonBlazor.UI.Filtering;
using CommonBlazor.UI.Filtering.Prefilters;
using CommonBlazor.UI.Filtering.QuickFilters;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;

namespace CommonBlazor.UI.List
{
    public abstract class EntityGridControllerBase<T> : GridController<T> where T : class
    {
        private IFilteringService? _filterigService;

        public IFilteringService FilteringService { get => _filterigService ?? throw ThrowHelper.InjectIsNull(); set => _filterigService = value; }

        public override DxGrid? Grid

        {
            get => State.Grid;
            set
            {
                State.Grid = value;

                if (value == null)
                {
                    DataSource = null;
                }
                else
                {
                    OnGridInitialized();
                }
            }
        }

        private readonly IMessenger _messenger;

        private IMessenger? _scopedMessenger;
        public IMessenger ScopedMessenger { get => _scopedMessenger ?? throw ThrowHelper.InjectIsNull(); set => _scopedMessenger = value; }

        private GridColumnRenderer<T>? _gridColumnRenderer;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText == value)
                    return;

                _searchText = value;
                RaiseStateHasChanged();
            }
        }

        public string KeyFieldName
        {
            get => _keyFieldName;
            set => _keyFieldName = ConvertPropertyName(value);
        }

        private string _keyFieldName = "id";

        public GridCustomDataSourceBase? DataSource { get; set; }

        public bool DataIsLoaded => DataSource != null && DataSource.DataIsLoaded;

        public FilterCriteria? Filter { get; set; }

        public Dictionary<string, FilterCriteria>? CollectionFilters
        {
            get => _collectionFilters;
            set => _collectionFilters = value;
        }

        public IEnumerable<FilterCriteria>? QuickFilters
        {
            get => _quickFilters;
            set => _quickFilters = value;
        }

        public IEnumerable<SortBySettings> SortBy { get; set; }

        protected string _searchText = string.Empty;
        protected FilterCriteria? _prefilter;
        protected Dictionary<string, FilterCriteria>? _collectionFilters;
        protected IEnumerable<FilterCriteria>? _quickFilters;
        
        public EntityContextBase EntityContext { get; set; }

        private bool _disposeScope;

        bool _refreshAfterGridInitialized = false;
        bool _applyFilteringAfterGridInitialize = false;
        bool _applySortingAfterGridInitialize = false;
        string? _firstColumnFilter;
        string? _sortByColumn;

        public EntityGridControllerBase(EntityContextBase entityContext, bool createScope = true) : base()
        {
            if (entityContext == null)
                throw new ArgumentNullException(nameof(entityContext));

            EntityContext = entityContext;

            if (createScope)
            {
                ServiceResolver.CreateScope(entityContext.ScopeId);
                _disposeScope = true;
            }

            State = ServiceResolver.Resolve<ListState>(entityContext.ScopeId);

            FilteringService = ServiceResolver.Resolve<IFilteringService>();

            _messenger = ServiceResolver.Resolve<IMessenger>();
            ScopedMessenger = ServiceResolver.Resolve<IMessenger>(entityContext.ScopeId);

            _refreshAfterGridInitialized = false;
        }

        public async Task InitializeAndRefreshAsync(bool initDefaultFilter, IEnumerable<GenericColumnSettings>? columns = null, CancellationToken cancellationToken = default)
        {
            await InitializeAsync(initDefaultFilter, columns, cancellationToken);

            RefreshData();
        }

        public virtual async Task InitializeAsync(bool initDefaultFilter, IEnumerable<GenericColumnSettings>? columns = null, CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return;

            await InitializeAsync(cancellationToken);
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            ScopedMessenger.Register<PrefilterChangedMessage>(this, OnFilterChanged);
            ScopedMessenger.Register<QuickFilterValuesChangedMessage>(this, OnQuickFiltersChanged);
            ScopedMessenger.Register<ColumnsChangedMessage>(this, OnColumnsChanged);

            _messenger.Register<ReloadGridMessage>(this, OnReloadGrid);

            if (EntityContext.VisibleProperties == null)
                throw new Exception("Columns are not initialized.");

            _gridColumnRenderer = new GridColumnRenderer<T>(this);
            _gridColumnRenderer.SetColumns(EntityContext.VisibleProperties);

            await base.InitializeAsync(cancellationToken);
        }
       
        public override object? GetSelectedValue(string field)
        {
            if (SelectedDataItemInternal == null || string.IsNullOrEmpty(field))
                return null;

            if (typeof(T) == typeof(ExpandoObject))
            {
                var obj = (ExpandoObject)Convert.ChangeType(SelectedDataItemInternal, typeof(ExpandoObject));
                obj.TryGetValue(field.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString(), out var key);
                return key;
            }
            else
            {
                return base.GetSelectedValue(field);
            }
        }

        public void RefreshData()
        {
            if (_applyFilteringAfterGridInitialize || _applySortingAfterGridInitialize)
            {
                _refreshAfterGridInitialized = true;
                return;
            }

            ClearDataSouce();

            DataSource = CreateDataSource();

            DataSource.AllDataRefreshed += DataSource_AllDataRefreshed;

            DataSource.Filter = Filter;
            DataSource.SortBy = SortBy;
            DataSource.CollectionFilters = _collectionFilters;
            DataSource.QuickFilters = _quickFilters;

            RaiseStateHasChanged();

            _refreshAfterGridInitialized = false;
        }

        protected abstract GridCustomDataSourceBase CreateDataSource();

        private void ClearDataSouce()
        {
            if (DataSource == null)
                return;

            DataSource.AllDataRefreshed -= DataSource_AllDataRefreshed;
            DataSource = null;
        }

        private void DataSource_AllDataRefreshed()
        {
            ClearSelection();
        }

        void OnColumnsChanged(ColumnsChangedMessage message)
        {
            if (message.Columns != null)
            {
                EntityContext.Properties = message.Columns;
                _gridColumnRenderer?.SetColumns(EntityContext.VisibleProperties);
            }

            RefreshData();
        }

        void OnFilterChanged(PrefilterChangedMessage message)
        {
            _prefilter = message.Filter;
            _collectionFilters = message.CollectionFilters;

            RefreshData();
        }

        void OnQuickFiltersChanged(QuickFilterValuesChangedMessage message)
        {
            _quickFilters = message.QuickFilters;

            RefreshData();
        }

        void OnReloadGrid(ReloadGridMessage message)
        {
            if (message.EntityName == EntityContext.EntityName)
            {
                ReloadData(false);
            }
        }

        public RenderFragment RenderGridColumns()
        {
            return _gridColumnRenderer?.RenderColumns();
        }

        public void ApplyFirstColumnFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return;

            if (Grid == null)
            {
                _applyFilteringAfterGridInitialize = true;
                _firstColumnFilter = filter;
                return;
            }

            var column = Grid.GetVisibleColumns().FirstOrDefault();

            if (column != null)
                Grid.FilterBy(ConvertPropertyName(column.Name), GridFilterRowOperatorType.Contains, filter);

            _applyFilteringAfterGridInitialize = false;
        }

        public void ApplySorting(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return;

            if (Grid == null)
            {
                _applySortingAfterGridInitialize = true;
                _sortByColumn = fieldName;
                return;
            }

            var column = Grid.GetVisibleColumns().FirstOrDefault(x => ConvertPropertyName(x.Name) == fieldName);

            if (column != null)
                Grid.SortBy(fieldName, GridColumnSortOrder.Ascending, 1);

            _applySortingAfterGridInitialize = false;
        }

        protected override void OnGridInitialized()
        {
            base.OnGridInitialized();

            if (_applySortingAfterGridInitialize && _sortByColumn != null)
                ApplySorting(_sortByColumn);

            if (_applyFilteringAfterGridInitialize && _firstColumnFilter != null)
                ApplyFirstColumnFilter(_firstColumnFilter);

            if (_refreshAfterGridInitialized)
                RefreshData();
        }

        public virtual string ConvertPropertyName(string propertyName)
        {
            return propertyName;
        }

        public override void Dispose()
        {
            ClearDataSouce();

            ScopedMessenger.UnregisterAll(this);

            if (_disposeScope)
                ServiceResolver.DisposeScope(EntityContext.ScopeId);

            base.Dispose();
        }
    }
}
