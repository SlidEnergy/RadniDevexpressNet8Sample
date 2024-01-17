using CommonBlazor.DynamicData;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.Extensions;
using CommonBlazor.Infrastructure;
using System.Dynamic;

namespace CommonBlazor.UI.List
{
    public class DynamicEntityGridController : EntityGridControllerBase<ExpandoObject>
    {
        public DynamicEntityContext DynamicEntityContext => (DynamicEntityContext)EntityContext;

        public DynamicEntityGridController(string entity, bool createScope = true) : base(new DynamicEntityContext(entity), createScope)
        {

        }

        public DynamicEntityGridController(DynamicEntityContext entityContext, bool createScope = true) : base(entityContext, createScope)
        {
        }

        public override async Task InitializeAsync(bool initDefaultFilter, IEnumerable<GenericColumnSettings>? columns = null, CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return;

            var tasks = new List<Task>();

            tasks.Add(InitializeEntityAsync(columns, cancellationToken));

            if (initDefaultFilter)
                tasks.Add(InitializeDefaultFilter(cancellationToken));

            await Task.WhenAll(tasks);

            await base.InitializeAsync(cancellationToken);
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return;

            await InitializeEntityAsync(null, cancellationToken);

            await base.InitializeAsync(cancellationToken);
        }

        private async Task InitializeDefaultFilter(CancellationToken cancellationToken = default)
        {
            var defaultFilter = await FilteringService.GetDefaultFilterAsync(((DynamicEntityContext)EntityContext).Key, cancellationToken);

            _collectionFilters = defaultFilter?.CriteriaCollection.Where(x => x.Value is not null).ToDictionary(x => x.Key, x => x.Value!);
        }

        private async Task InitializeEntityAsync(IEnumerable<GenericColumnSettings>? columns, CancellationToken cancellationToken = default)
        {
            await DynamicEntityContext.InitializeAsync(cancellationToken);

            if (columns != null)
                DynamicEntityContext.Properties = columns.ToList();
        }

        protected override GridCustomDataSourceBase CreateDataSource()
        {
            return new LoadOnDemandDynamicEntityDataSource<ExpandoObject>((DynamicEntityContext)EntityContext, this);
        }

        public override string ConvertPropertyName(string propertyName)
        {
            return propertyName.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString();
        }

        public override void ReloadData(bool refreshFromServer = true)
        {
            if (Grid == null) return;
            ClearSelectionInternal();
            ((LoadOnDemandDynamicEntityDataSource<ExpandoObject>)DataSource).RefreshFromServer = refreshFromServer;
            Grid.Reload();
            //CommandManager?.Update();
        }
    }

    public class DynamicEntityGridController<TDataItem> : DynamicEntityGridController where TDataItem : class
    {
        public TDataItem? SelectedDynamicDataItem { get => GetSelectedDataItem<TDataItem>(); set => State.SelectedDataItem = value; }

        public List<TDataItem> SelectedDynamicDataItems { get => GetSelectedDataItems<TDataItem>(); set => State.SelectedDataItem = value; }

        private event Action<TDataItem>? _dynamicDataItemChoosed;

        public event Action<TDataItem> DynamicDataItemChoosed
        {
            add
            {
                DataItemChoosed += DynamicEntityGridController_DataItemChoosed;
                _dynamicDataItemChoosed += value;
            }
            remove
            {
                DataItemChoosed -= DynamicEntityGridController_DataItemChoosed;
                _dynamicDataItemChoosed -= value;
            }
        }

        public DynamicEntityGridController(string entity, bool createScope = true) : base(entity, createScope)
        {

        }

        public DynamicEntityGridController(DynamicEntityContext entityContext) : base(entityContext)
        {

        }

        private void DynamicEntityGridController_DataItemChoosed(ExpandoObject obj)
        {
            _dynamicDataItemChoosed?.Invoke(obj.ConvertTo<TDataItem>(true));
        }

        public override TData? GetSelectedDataItem<TData>() where TData : class
        {
            if (typeof(TData) == typeof(ExpandoObject))
                return base.GetSelectedDataItem<TData>();

            return base.GetSelectedDataItem<ExpandoObject>()?.ConvertTo<TData>(true);
        }

        public override List<TData> GetSelectedDataItems<TData>() where TData : class
        {
            if (typeof(TData) == typeof(ExpandoObject))
                return base.GetSelectedDataItems<TData>();

            return base.GetSelectedDataItems<ExpandoObject>().Select(x => x.ConvertTo<TData>(true)).ToList();
        }

        public override TData? GetDataItem<TData>(int visibleIndex) where TData : class
        {
            if (typeof(TData) == typeof(ExpandoObject))
                base.GetDataItem<TData>(visibleIndex);

            return base.GetDataItem<ExpandoObject>(visibleIndex)?.ConvertTo<TData>(true);
        }

        public override void SelectDataItem(object dataItem)
        {
            if (Grid == null)
                throw new InvalidOperationException("Grid is not initialized");

            Grid.SelectDataItem(dataItem.ToExpandoObject(true));
        }

        public override void SelectDataItems(IEnumerable<object> dataItems)
        {
            if (Grid == null)
                throw new InvalidOperationException("Grid is not initialized");

            Grid.SelectDataItems(dataItems.Select(x => x.ToExpandoObject(true)).ToList());
        }

        public override void DeselectDataItem(object dataItem)
        {
            if (Grid == null)
                throw new InvalidOperationException("Grid is not initialized");

            Grid.DeselectDataItem(dataItem.ToExpandoObject(true));
        }

        public override void DeselectDataItems(IEnumerable<object> dataItems)
        {
            if (Grid == null)
                throw new InvalidOperationException("Grid is not initialized");

            Grid.DeselectDataItems(dataItems.Select(x => x.ToExpandoObject(true)).ToList());
        }
    }
}
