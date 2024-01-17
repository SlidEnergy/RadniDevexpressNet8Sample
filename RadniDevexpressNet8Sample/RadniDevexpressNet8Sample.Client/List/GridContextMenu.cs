using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonBlazor.UI.List
{
    public class GridContextMenu
    {
        public DxContextMenu? ContextMenu { get; set; }

        public ListState? State { get; set; }

        public void AddContextMenuCustomizeElement(GridCustomizeElementEventArgs e)
        {
            if (ContextMenu == null || State == null)
                return;

            if (e.ElementType == GridElementType.DataCell && e.Column is DxGridDataColumn)
            {
                e.Attributes["oncontextmenu"] = EventCallback.Factory.Create(this, (Func<MouseEventArgs, Task>)onContextMenu);

                async Task onContextMenu(MouseEventArgs evargs)
                {
                    State.ContextMenuDataItem = e.Grid.GetDataItem(e.VisibleIndex);
                    State.ContextMenuCellValue = e.Grid.GetDataItemValue(State.ContextMenuDataItem, (e.Column as DxGridDataColumn)!.FieldName);

                    e.Grid.DeselectDataItems(State.SelectedDataItems);
                    e.Grid.SelectRow(e.VisibleIndex, true);

                    await ContextMenu.ShowAsync(evargs);
                }
            }
        }
    }
}
