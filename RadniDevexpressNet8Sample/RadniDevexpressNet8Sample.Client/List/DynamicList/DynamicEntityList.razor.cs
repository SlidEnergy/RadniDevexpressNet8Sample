using System.Dynamic;
using Microsoft.AspNetCore.Components;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.Extensions;

namespace CommonBlazor.UI.List.DynamicList
{
    public partial class DynamicEntityList<TData> : ApplicationComponentBase where TData : class
    {
        [Parameter]
        public EventCallback<TData> RowDoubleClick { get; set; }

        [Parameter]
        public EventCallback<TData> SelectedDataItemChanged { get; set; }

        [Parameter]
        public EventCallback<SelectedDataItemsChangedEventArgs<TData>> SelectedDataItemsChanged { get; set; }

        [Parameter]
        public EntityGridControllerBase<ExpandoObject>? Controller { get; set; }

        [Parameter]
        public IEnumerable<GenericColumnSettings>? Columns { get; set; }

        [Parameter]
        public string? CssClass { get; set; }

        [Parameter]
        public string? GridCssClass { get; set; }

        [Parameter]
        public bool ShowSearchBox { get; set; }

        [Parameter] public bool? VirtualScrollingEnabled { get; set; }

        [Parameter] public int PageSize { get; set; }

        public override bool UseParametersChangeChecking => true;

        void OnRowDoubleClick(ExpandoObject obj)
        {
            var model = obj.ConvertTo<TData>(true);
            RowDoubleClick.InvokeAsync(model);
        }

        void OnSelectedDataItemChanged(ExpandoObject obj)
        {
            SelectedDataItemChanged.InvokeAsync(obj.ConvertTo<TData>(true));
        }

        void OnSelectedDataItemsChanged(SelectedDataItemsChangedEventArgs<ExpandoObject> e)
        {
            var convertedEventArgs = new SelectedDataItemsChangedEventArgs<TData>()
            {
                NewSelection = e.NewSelection.Select(x => x.ConvertTo<TData>(true)).ToList(),
                OldSelection = e.OldSelection.Select(x => x.ConvertTo<TData>(true)).ToList(),
                DataItemsSelected = e.DataItemsSelected.Select(x => x.ConvertTo<TData>(true)).ToList(),
                DataItemsDeselected = e.DataItemsDeselected.Select(x => x.ConvertTo<TData>(true)).ToList()
            };

            SelectedDataItemsChanged.InvokeAsync(convertedEventArgs);
        }
    }
}