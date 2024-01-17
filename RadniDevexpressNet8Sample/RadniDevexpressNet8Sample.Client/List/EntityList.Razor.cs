using System.Dynamic;
using CommonBlazor.DynamicData;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.Infrastructure;
using CommonBlazor.UI.Configuration;
using CommonBlazor.UI.Util;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;

namespace CommonBlazor.UI.List
{
    public partial class EntityList<TData> : ApplicationComponentBase, IDisposable where TData : class
    {
        public DxGrid Grid
        {
            get => _grid ?? throw ThrowHelper.ComponentReferenceIsNull();
            set
            {
                _grid = value;
                Controller.Grid = value;
            }
        }

        protected GlobalCommonConfiguration _globalConfiguration;
        protected GlobalCommonConfiguration GlobalConfiguration
        {
            get
            {
                if (_globalConfiguration == null)
                    _globalConfiguration = ServiceResolver.Resolve<GlobalCommonConfiguration>() ?? throw ThrowHelper.InjectIsNull();

                return _globalConfiguration;
            }
        }

        [Parameter]
        public EventCallback<TData> RowDoubleClick { get; set; }

        [Parameter]
        public EventCallback<TData> SelectedDataItemChanged { get; set; }

        [Parameter]
        public EventCallback<SelectedDataItemsChangedEventArgs<TData>> SelectedDataItemsChanged { get; set; }

        [Parameter]
        public TData? SelecteDataItem { get => Controller.GetSelectedDataItem<TData>(); set => Controller.SelectedDataItemInternal = value; }

        [Parameter]
        public IEnumerable<GenericColumnSettings>? Columns { get; set; }

        [Parameter]
        public EntityGridControllerBase<TData> Controller { get => _controller ?? throw ThrowHelper.ParameterIsNull(); set => _controller = value; }

        [Parameter]
        public string? CssClass
        {
            set
            {
                if (value == _cssClass)
                    return;

                _cssClass = value;

                _containerGridCssClass = CssUtil.CombineCssClasses("grid-container", value);
                _containerSpinnerCssClass = CssUtil.CombineCssClasses("spinner-container", value);
            }
        }

        private string? _cssClass;

        private string? _containerGridCssClass = "grid-container";
        private string? _containerSpinnerCssClass = "spinner-container";

        [Parameter]
        public string? GridCssClass { get; set; }

        [Parameter] public int PageSize { get; set; } = 20;

        [Parameter] public bool ShowSearchBox { get; set; } = false;

        [Parameter] public bool? VirtualScrollingEnabled { get; set; }

        public override bool UseParametersChangeChecking => true;
        public override bool ManualRenderStrategy => true;

        private DxContextMenu? _contextMenu;
        private DxGrid? _grid;
        private EntityGridControllerBase<TData>? _controller;
        private DynamicEntityContext? _entityContext;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _emptyRendered = false;
        private bool _initialized = false;

        private bool IsDynamicEntity
        {
            get => typeof(TData) == typeof(ExpandoObject);
        }

        public DxContextMenu ContextMenu
        {
            get => _contextMenu ?? throw ThrowHelper.ComponentReferenceIsNull();
            set
            {
                _contextMenu = value;
                Controller.ContextMenu = value;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (VirtualScrollingEnabled == null)
                VirtualScrollingEnabled = GlobalConfiguration?.GridVirtualScrollEnabled;

            Controller.StateHasChanged += Controller_StateHasChanged;
            Controller.SelectedDataItemChanged += Controller_DataItemSelected;
            Controller.SelectedDataItemsChanged += Controller_DataItemsSelected;
            Controller.SelectionModeChanged += Controller_SelectionModeChanged;

            await Controller.InitializeAsync(true, Columns, _cancellationTokenSource.Token);

            _initialized = true;

            Controller.RefreshData();

            await InvokeStateHasChanged();
        }

        private async void Controller_SelectionModeChanged()
        {
            await InvokeStateHasChanged();
        }

        private void Controller_DataItemSelected(TData? obj)
        {
            SelectedDataItemChanged.InvokeAsync(obj);
        }

        private void Controller_DataItemsSelected(SelectedDataItemsChangedEventArgs<TData> e)
        {
            SelectedDataItemsChanged.InvokeAsync(e);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // At first fast render empty content even though we can render all content syncronously.
                await Task.Run(async () =>
                {
                    _emptyRendered = true;
                    await InvokeStateHasChanged();
                });
            }
        }

        private async void Controller_StateHasChanged()
        {
            await InvokeStateHasChanged();
        }

        private void OnRowDoubleClick(GridRowClickEventArgs e)
        {
            RowDoubleClick.InvokeAsync(Controller.GetSelectedDataItem<TData>());
        }

        public void Dispose()
        {
            if (_controller != null)
            {
                _controller.StateHasChanged -= Controller_StateHasChanged;
                _controller.SelectedDataItemChanged -= Controller_DataItemSelected;
                _controller.SelectionModeChanged -= Controller_SelectionModeChanged;
                _controller.SelectedDataItemsChanged -= Controller_DataItemsSelected;
            }
        }
    }
}

