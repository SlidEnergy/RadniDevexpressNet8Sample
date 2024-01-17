using CommonBlazor.DynamicData;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.UI.Shared;
using Microsoft.AspNetCore.Components;

namespace CommonBlazor.UI.List
{
    internal class GridColumnRenderer<T> where T : class
    {
        private readonly EntityGridControllerBase<T>? _controller;
        IEnumerable<GenericColumnSettings>? _columns;
        bool _renderFragment = true;
        RenderFragment? _cachedRenderFragment;

        public GridColumnRenderer(EntityGridControllerBase<T> controller)
        {
            _controller = controller;
        }

        public RenderFragment RenderColumns()
        {
            if (_renderFragment || _cachedRenderFragment == null)
            {
                _cachedRenderFragment = TypedGridColumnRenderer.RenderTypedColumns(GenerateGridColumnModels(), _controller as DynamicEntityGridController);
                _renderFragment = false;
            }

            return _cachedRenderFragment;
        }

        public void SetColumns(IEnumerable<GenericColumnSettings> columns)
        {
            _columns = columns;
            _renderFragment = true;
        }

        private List<TypedGridColumnModel> GenerateGridColumnModels()
        {
            var items = new List<TypedGridColumnModel>();

            if (_columns != null && _controller != null)
            {
                foreach (var property in _columns)
                {
                    var item = new TypedGridColumnModel()
                    {
                        ColumnType = property.ParsedType,
                        FieldName = _controller.ConvertPropertyName(property.FullPropertyName),
                        Name = property.FullPropertyName,
                        Caption = property.DisplayName,
                        DisplayFormat = property.DisplayFormat,
                        Width = property.ColumnWidth == null ? null : property.ColumnWidth + "px",
                        ColumnSettings = property
                    };

                    items.Add(item);
                }
            }

            return items;
        }
    }
}
