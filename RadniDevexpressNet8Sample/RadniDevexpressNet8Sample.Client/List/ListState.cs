using CommonBlazor.DynamicData.Models;
using DevExpress.Blazor;

namespace CommonBlazor.UI.List
{
    public class ListState
    {
        public bool IsInitialized { get; set; }

        public object? ContextMenuDataItem { get; set; }

        public object? ContextMenuCellValue { get; set; }

        public IReadOnlyList<object> SelectedDataItems { get; set; } = new List<object>();

        public int SelectedPageIndex { get; set; } = 0;

        public object? SelectedDataItem { get; set; }

        public DxGrid? Grid{ get; set; }

        public bool IsRowEditing { get; set; }
    }
}
