using Microsoft.AspNetCore.Components;
using DevExpress.Blazor;
using CommonBlazor.Infrastructure;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.UI.List;
using CommonBlazor.UI.Util;
using DevExpress.Blazor.Internal;

namespace CommonBlazor.UI.Filtering.HeaderFilter
{
    public partial class HeaderFilterEntityList<TData> : ApplicationComponentBase, IDisposable where TData : class
    {
        private DxGrid? _grid { get; set; }

        private EntityGridControllerBase<TData>? _controller;

        [Parameter]
        public EntityGridControllerBase<TData> Controller { get => _controller ?? throw ThrowHelper.ParameterIsNull(); set => _controller = value; }

        [Parameter]
        public IEnumerable<GenericColumnSettings>? Columns { get; set; }

        [Parameter] public string GridCssClass { get; set; } = "";

        public override bool UseParametersChangeChecking => true;

        public override bool ManualRenderStrategy => true;

        private bool _emptyRendered = false;

        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await Controller.InitializeAndRefreshAsync(false, Columns, _cancellationTokenSource.Token);
            Controller.StateHasChanged += Controller_StateHasChanged;

            await InvokeStateHasChanged();
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

            // Initialize grid after render columns
            if (Controller.Grid == null && _grid != null && _grid.GetVisibleColumns().Any())
            {
                Controller.Grid = _grid;
            }
        }

        private async void Controller_StateHasChanged()
        {
            await InvokeStateHasChanged();
        }

        string GetCombinedCssClass(string cssClass)
        {
            return CssUtil.CombineCssClasses(cssClass, GridCssClass ?? "");
        }

        public void Dispose()
        {
            if (_controller != null)
                _controller.StateHasChanged -= Controller_StateHasChanged;
        }
    }
}
