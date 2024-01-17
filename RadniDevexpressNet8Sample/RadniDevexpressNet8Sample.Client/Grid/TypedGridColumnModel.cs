using CommonBlazor.DynamicData;
using CommonBlazor.DynamicData.Models;
using DevExpress.Blazor;

namespace CommonBlazor.UI.Shared
{
    public class TypedGridColumnModel
    {
        public Type ColumnType { get; set; }
        public string? FieldName { get; set; }
        public string? Name { get; set; }
        public string? Caption { get; set; }
        public string? DisplayFormat { get; set; }

        public string? Width { get; set; }

        public GridUnboundColumnType UnboundType { get; set; } = GridUnboundColumnType.Bound;

        public GenericColumnSettings? ColumnSettings { get; set; }
    }
}
