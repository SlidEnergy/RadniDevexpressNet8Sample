using AndromedaBlazor.ViewModel;
using CommonBlazor.Infrastructure;
using CommonBlazor.UI;
using CommonBlazor.UI.List;
using Microsoft.AspNetCore.Components;

namespace RadniDevexpressNet8Sample.client.Pages
{
    public partial class RadniNalogList : ApplicationComponentBase, IDisposable
    {
        //private UICommandManager? _uICommandManager;

        //[Inject]
        //public UICommandManager UICommandManager { get => _uICommandManager ?? throw ThrowHelper.InjectIsNull(); set => _uICommandManager = value; }

        DynamicEntityGridController<RadniNalogListData> _controller = new DynamicEntityGridController<RadniNalogListData>("RadniNalog");

        private List<string> filter;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //_controller.CommandManager = UICommandManager;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                //UICommandManager
                //    //.AddNewModel(OnNewModelAsync, new UICommandOption() { CssClass = "andromeda-button-add", Order = 0 })
                //    //.AddEditModel(OnEditModelAsync, _controller, new UICommandOption() { CssClass = "andromeda-button-edit", BeginGroup = true, Order = 1 })
                //    //.AddDeleteModel(OnDeleteModelAsync, _controller, new UICommandOption() { CssClass = "andromeda-button-delete", Order = 2, BeginGroup = true })
                //    .AddToToolbar(UICommandFactory.Refresh(), OnRefresh, new UICommandOption() { CssClass = "andromeda-button-refresh", BeginGroup = true, Order = 3 })
                //    .OnStateHasChanged(() => InvokeAsync(StateHasChanged))
                //    .Update();
            }
        }

        //async Task OnNewModelAsync(CancellationToken cancellationToken)
        //{
        //    NavigationService.NavigateTo("/radniNalogEditor");
        //}

        //async Task OnEditModelAsync(CancellationToken cancellationToken)
        //{
        //    var selectedKalkulacija = _controller.GetSelectedDataItem<KalkulacijaListData>();
        //    if (selectedKalkulacija == null) return;
        //    NavigationService.NavigateTo("/radniNalogEditor/" + selectedKalkulacija.Id);
        //}

        //async Task OnDeleteModelAsync(CancellationToken cancellationToken)
        //{
        //    var id = _controller.GetSelectedValue(nameof(RadniNalogListData.Id));
        //    if (id == null) return;
        //    var rowVersion = _controller.GetSelectedValue(nameof(RadniNalogListData.RowVersion));
        //    await TaskProvider.ExecuteAsync(async () =>
        //    {
        //        var request = new RadniNalogDeleteRequest() { Id = (long)id, RowVersion = (int)rowVersion };
        //        await Dispatcher.Send(request, cancellationToken);
        //    }, Localize["OperationInProgress"]);
        //    _controller.ReloadData();
        //}

        async Task OnRefresh(CancellationToken cancellationToken)
        {
            if (_controller == null) return;
            _controller.ReloadData();
        }

        private async Task OnRowDoubleClick(RadniNalogListData dataItem)
        {
            //NavigationService.NavigateTo("/radniNalogEditor/" + dataItem.Id);
        }

        public override void Dispose()
        {
            base.Dispose();

            _controller.Dispose();
        }
    }
}
