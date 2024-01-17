using System.Dynamic;
using System.Reflection;
using Common.DataAccess.Filtering;
using Common.Windows.Utils;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using DevExpress.Blazor;
using CommonBlazor.UI.Components;
using CommonBlazor.UI.Components.Validation;
using CommonBlazor.UI.List;
using DevExpress.Data.Filtering;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraPrinting.Native;
using CommonBlazor.UI.Filtering.HeaderFilter;
using DevExpress.Blazor.Internal;
using CommonBlazor.DynamicData;
using CommonBlazor.Infrastructure;

namespace CommonBlazor.UI.Shared
{
    public class TypedGridColumnRenderer
    {
        public static RenderFragment RenderTypedColumn(TypedGridColumnModel column)
        {
            RenderFragment render = b =>
            {
                RenderTypedColumnInternal(column, b, null);
            };

            return render;
        }

        public static RenderFragment RenderTypedColumns(IEnumerable<TypedGridColumnModel> columns, DynamicEntityGridController? controller = null)
        {
            RenderFragment render = b =>
            {
                foreach (var column in columns)
                {
                    RenderTypedColumnInternal(column, b, controller);
                }
            };

            return render;
        }

        private static void RenderTypedColumnInternal(TypedGridColumnModel column, RenderTreeBuilder b, DynamicEntityGridController? controller)
        {
            b.OpenComponent(0, typeof(DxGridDataColumn));
            b.AddAttribute(0, "Name", column.Name);
            b.AddAttribute(0, "FieldName", column.FieldName);
            b.AddAttribute(0, "Caption", column.Caption);
            b.AddAttribute(0, "DisplayFormat", column.DisplayFormat);

            if (column.Width != null)
                b.AddAttribute(0, "Width", column.Width);

            if (column.UnboundType != GridUnboundColumnType.Bound)
                b.AddAttribute(0, "UnboundType", column.UnboundType);

            if(column.ColumnType == typeof(string))
                b.AddAttribute(0, "FilterRowOperatorType", GridFilterRowOperatorType.Contains);

            if(controller != null)
                AddFilterMenuTemplate(column, b, controller);

            AddFilterRowCellTemplate(column, b);

            AddCellDisplayTemplate(column, b);

            b.CloseComponent();
        }

        static void AddFilterRowCellTemplate(TypedGridColumnModel column, RenderTreeBuilder b)
        {
            if (column.ColumnType == typeof(Boolean) || column.ColumnType == typeof(Boolean?))
            {
                b.AddAttribute(0, "FilterRowCellTemplate", (RenderFragment<GridDataColumnFilterRowCellTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenElement(0, "div");
                    b2.AddAttribute(0, "style", "margin: 0 auto; width: 24px");
                    b2.OpenComponent(0, typeof(DxCheckBox<bool?>));
                    b2.AddAttribute(0, "CheckedChanged", EventCallback.Factory.Create(b, (bool? v) => context.FilterRowValue = v));
                    b2.AddAttribute(0, "AllowIndeterminateState", true);
                    b2.AddAttribute(0, "AllowIndeterminateStateByClick", true);
                    b2.CloseComponent();
                    b2.CloseElement();
                }));
            }
            else if (column.ColumnType == typeof(DateTime) || column.ColumnType == typeof(DateTime?))
            {
                b.AddAttribute(0, "FilterRowCellTemplate", (RenderFragment<GridDataColumnFilterRowCellTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenComponent(0, typeof(DxDateEdit<DateTime>));
                    b2.AddAttribute(0, "DateChanged", EventCallback.Factory.Create(b, (DateTime date) => context.FilterRowValue = date == DateTime.MinValue ? null : date));
                    b2.AddAttribute(0, "NullText", "");
                    b2.AddAttribute(0, "NullValue", DateTime.MinValue);
                    b2.AddAttribute(0, "MinDate", new DateTime(1900, 1, 1));
                    b2.AddAttribute(0, "MaxDate", new DateTime(2099, 12, 31));
                    b2.AddAttribute(0, "ClearButtonDisplayMode", DataEditorClearButtonDisplayMode.Auto);
                    b2.CloseComponent();
                }));
            }
            else
            {
                b.AddAttribute(0, "FilterRowCellTemplate", (RenderFragment<GridDataColumnFilterRowCellTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenComponent(0, typeof(DxTextBox));
                    b2.AddAttribute(0, "BindValueMode", BindValueMode.OnDelayedInput);
                    b2.AddAttribute(0, "ClearButtonDisplayMode", DataEditorClearButtonDisplayMode.Auto);
                    b2.AddAttribute(0, "TextChanged", EventCallback.Factory.Create(b, (string text) => context.FilterRowValue = text));
                    b2.CloseComponent();
                }));
            }
        }

        static void AddCellDisplayTemplate(TypedGridColumnModel column, RenderTreeBuilder b)
        {
            if (column.ColumnType == typeof(Boolean) || column.ColumnType == typeof(Boolean?))
            {
                b.AddAttribute(0, "CellDisplayTemplate", (RenderFragment<GridDataColumnCellDisplayTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenComponent(0, typeof(DxCheckBox<bool?>));
                    b2.AddAttribute(0, "Enabled", false);
                    b2.AddAttribute(0, "Checked", (bool)context.Value);
                    b2.CloseComponent();
                }));
            }
        }

        static void AddFilterMenuTemplate(TypedGridColumnModel column, RenderTreeBuilder b, DynamicEntityGridController controller)
        {
            if (column.ColumnSettings == null)
                return;

            var dynamicEntityKey = controller.DynamicEntityContext.Key;
            
            b.AddAttribute(0, "FilterMenuTemplate", (RenderFragment<GridDataColumnFilterMenuTemplateContext>)(context =>
                b2 =>
                {
                    var fieldName = context.DataColumn.FieldName;

                    var headerFilterGridController = new HeaderFilterDynamicEntityGridController(dynamicEntityKey, context.DataColumn.FieldName);
                    headerFilterGridController.SelectedDataItemsChanged += e =>
                    {
                        if (column.ColumnType == typeof(DateTime) || column.ColumnType == typeof(DateTime?))
                        {
                            context.FilterCriteria = new FunctionOperator(FunctionOperatorType.IsSameDay,
                                e.NewSelection
                                    .Select(v => (CriteriaOperator)new OperandValue(v.GetValue(fieldName)))
                                    .Prepend((CriteriaOperator)new OperandProperty(fieldName)).ToList());
                            return;
                        }

                        context.FilterCriteria = new InOperator(new OperandProperty(fieldName),
                            e.NewSelection.Select(v => new OperandValue(v.GetValue(fieldName))).ToList());
                    };
                    headerFilterGridController.KeyFieldName = fieldName;
                    headerFilterGridController.SelectionMode = GridSelectionMode.Multiple;

                    var converter = new DXCriteriaOperatorConverter();
                    var gridFilter = converter.Convert(context.Grid.GetFilterCriteria());

                    var propertyNameResolveProvider = ServiceResolver.Resolve<IPropertyNameResolveProvider>();
                    var propertyNameResolver = propertyNameResolveProvider.GetUpperCasePropertyNameResolver(controller.EntityContext.Properties);

                    propertyNameResolver.ResolvePropertyNames(gridFilter);

                    if (controller.Filter != null && gridFilter != null)
                    {
                        headerFilterGridController.Filter = CriteriaFactory.And(controller.Filter, gridFilter);
                    }
                    else
                    {
                        if (controller.Filter != null)
                            headerFilterGridController.Filter = controller.Filter;

                        if (gridFilter != null)
                            headerFilterGridController.Filter = gridFilter;
                    }

                    headerFilterGridController.QuickFilters = controller.QuickFilters;
                    headerFilterGridController.CollectionFilters = controller.CollectionFilters;

                    b2.OpenComponent(0, typeof(HeaderFilterEntityList<ExpandoObject>));
                    b2.AddAttribute(0, "Controller", headerFilterGridController);
                    b2.AddAttribute(0, "Columns", new List<GenericColumnSettings> { new GenericColumnSettings()
                    {
                        FullPropertyName = column.ColumnSettings.FullPropertyName,
                        ParsedType = column.ColumnType,
                        DisplayName = column.Caption,
                        AddToGrid = true,
                        Visible = true,
                        IsPrimaryKey = false
                    } });
                    b2.CloseComponent();
                }));
        }

        public static RenderFragment RenderTypedColumnWithValidation(TypedGridColumnModel column, string collectionFieldName, Type modelType)
        {
            RenderFragment render = b =>
            {
                RenderTypedColumnWithValidationInternal(column, collectionFieldName, modelType, b);
            };

            return render;
        }

        private static void RenderTypedColumnWithValidationInternal(TypedGridColumnModel column, string collectionFieldName, Type modelType, RenderTreeBuilder b)
        {
            b.OpenComponent(0, typeof(DxGridDataColumn));
            b.AddAttribute(0, "Name", column.Name);
            b.AddAttribute(0, "FieldName", column.FieldName);
            b.AddAttribute(0, "Caption", column.Caption);
            b.AddAttribute(0, "DisplayFormat", column.DisplayFormat);

            if (column.Width != null)
                b.AddAttribute(0, "Width", column.Width);

            if (column.UnboundType != GridUnboundColumnType.Bound)
                b.AddAttribute(0, "UnboundType", column.UnboundType);

            if (column.ColumnType == typeof(string))
                b.AddAttribute(0, "FilterRowOperatorType", GridFilterRowOperatorType.Contains);

            if (column.ColumnType == typeof(Boolean) || column.ColumnType == typeof(Boolean?))
            {
                b.AddAttribute(0, "FilterRowCellTemplate", (RenderFragment<GridDataColumnFilterRowCellTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenElement(0, "div");
                    b2.AddAttribute(0, "style", "margin: 0 auto; width: 24px");
                    b2.OpenComponent(0, typeof(DxCheckBox<bool?>));
                    b2.AddAttribute(0, "CheckedChanged", EventCallback.Factory.Create(b, (bool? v) => context.FilterRowValue = v));
                    b2.AddAttribute(0, "AllowIndeterminateState", true);
                    b2.AddAttribute(0, "AllowIndeterminateStateByClick", true);
                    b2.CloseComponent();
                    b2.CloseElement();
                }));

                b.AddAttribute(0, "CellEditTemplate", (RenderFragment<GridDataColumnCellEditTemplateContext>)((context) => (b2) =>
                {
                    var model = context.EditModel;

                    var property = modelType.GetProperty(column.FieldName, BindingFlags.Public | BindingFlags.Instance);

                    b2.OpenComponent(0, typeof(DxCheckBox<bool>));
                    b2.AddAttribute(0, "CssClass", ValidationClasses.ForEditor(column.FieldName));
                    b2.AddAttribute(0, "AllowIndeterminateState", true);
                    b2.AddAttribute(0, "AllowIndeterminateStateByClick", true);
                    b2.AddAttribute(0, "Checked", property.GetValue(model));
                    b2.AddAttribute(0, "CheckedChanged", EventCallback.Factory.Create(b, (bool? v) => property.SetValue(model, v)));
                    b2.CloseComponent();
                }));
            }
            else if (column.ColumnType == typeof(DateTime) || column.ColumnType == typeof(DateTime?))
            {
                b.AddAttribute(0, "FilterRowCellTemplate", (RenderFragment<GridDataColumnFilterRowCellTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenComponent(0, typeof(DxDateEdit<DateTime>));
                    b2.AddAttribute(0, "DateChanged", EventCallback.Factory.Create(b, (DateTime date) => context.FilterRowValue = date == DateTime.MinValue ? null : date));
                    b2.AddAttribute(0, "NullText", "");
                    b2.AddAttribute(0, "NullValue", DateTime.MinValue);
                    b2.AddAttribute(0, "MinDate", new DateTime(1900, 1, 1));
                    b2.AddAttribute(0, "MaxDate", new DateTime(2099, 12, 31));
                    b2.AddAttribute(0, "ClearButtonDisplayMode", DataEditorClearButtonDisplayMode.Auto);
                    b2.CloseComponent();
                }));

                b.AddAttribute(0, "CellEditTemplate", (RenderFragment<GridDataColumnCellEditTemplateContext>)((context) => (b2) =>
                {
                    var model = context.EditModel;

                    var property = modelType.GetProperty(column.FieldName, BindingFlags.Public | BindingFlags.Instance);

                    b2.OpenComponent(0, typeof(DxDateEdit<DateTime>));
                    b2.AddAttribute(0, "CssClass", ValidationClasses.ForEditor(column.FieldName));
                    b2.AddAttribute(0, "NullText", "");
                    b2.AddAttribute(0, "NullValue", DateTime.MinValue);
                    b2.AddAttribute(0, "MinDate", new DateTime(1900, 1, 1));
                    b2.AddAttribute(0, "MaxDate", new DateTime(2099, 12, 31));
                    b2.AddAttribute(0, "ClearButtonDisplayMode", DataEditorClearButtonDisplayMode.Auto);
                    b2.AddAttribute(0, "Date", property.GetValue(model));
                    b2.AddAttribute(0, "DateChanged", EventCallback.Factory.Create(b, (DateTime date) => property.SetValue(model, date == DateTime.MinValue ? null : date)));
                    b2.CloseComponent();
                }));
            }
            else if (column.ColumnType.IsNumeric())
            {
                b.AddAttribute(0, "FilterRowCellTemplate", (RenderFragment<GridDataColumnFilterRowCellTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenComponent(0, typeof(InputBindModeTextBox));
                    b2.AddAttribute(0, "TextChanged", EventCallback.Factory.Create(b, (string text) => context.FilterRowValue = text));
                    b2.CloseComponent();
                }));

                b.AddAttribute(0, "CellEditTemplate", (RenderFragment<GridDataColumnCellEditTemplateContext>)((context) => (b2) =>
                {
                    var model = context.EditModel;

                    var property = modelType.GetProperty(column.FieldName, BindingFlags.Public | BindingFlags.Instance);

                    if (column.ColumnType.IsIntegral())
                    {
                        b2.OpenComponent(0, typeof(DxSpinEdit<long>));
                        b2.AddAttribute(0, "CssClass", ValidationClasses.ForEditor(column.FieldName));
                        b2.AddAttribute(0, "Value", property.GetValue(model));
                        b2.AddAttribute(0, "ValueChanged",
                            EventCallback.Factory.Create(b, (long value) => property.SetValue(model, value)));
                        b2.CloseComponent();
                    }
                    else if (column.ColumnType.IsFloatingPoint() || column.ColumnType.IsDecimal())
                    {
                        b2.OpenComponent(0, typeof(DxSpinEdit<double>));
                        b2.AddAttribute(0, "CssClass", ValidationClasses.ForEditor(column.FieldName));
                        b2.AddAttribute(0, "Value", property.GetValue(model));
                        b2.AddAttribute(0, "ValueChanged",
                            EventCallback.Factory.Create(b, (double value) => property.SetValue(model, value)));
                        b2.CloseComponent();
                    }
                }));
            }
            else
            {
                b.AddAttribute(0, "FilterRowCellTemplate", (RenderFragment<GridDataColumnFilterRowCellTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenComponent(0, typeof(InputBindModeTextBox));
                    b2.AddAttribute(0, "TextChanged", EventCallback.Factory.Create(b, (string text) => context.FilterRowValue = text));
                    b2.CloseComponent();
                }));

                b.AddAttribute(0, "CellEditTemplate", (RenderFragment<GridDataColumnCellEditTemplateContext>)((context) => (b2) =>
                {
                    var model = context.EditModel;

                    var property = modelType.GetProperty(column.FieldName, BindingFlags.Public | BindingFlags.Instance);

                    b2.OpenComponent(0, typeof(DxTextBox));
                    b2.AddAttribute(0, "CssClass", ValidationClasses.ForEditor(column.FieldName));
                    b2.AddAttribute(0, "Text", property.GetValue(model));
                    b2.AddAttribute(0, "TextChanged", EventCallback.Factory.Create(b, (string text) => property.SetValue(model, text)));
                    b2.CloseComponent();
                }));
            }

            if (column.ColumnType == typeof(Boolean) || column.ColumnType == typeof(Boolean?))
            {
                b.AddAttribute(0, "CellDisplayTemplate", (RenderFragment<GridDataColumnCellDisplayTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenElement(0, "div");
                    b2.AddAttribute(0, "class", ValidationClasses.ForGridCell($"{collectionFieldName}_{context.VisibleIndex}_{column.FieldName}"));
                    b2.OpenComponent(0, typeof(DxCheckBox<bool?>));
                    b2.AddAttribute(0, "Enabled", false);
                    b2.AddAttribute(0, "Checked", (bool)context.Value);
                    b2.CloseComponent();
                    b2.CloseElement();
                }));
            }
            else
            {
                b.AddAttribute(0, "CellDisplayTemplate", (RenderFragment<GridDataColumnCellDisplayTemplateContext>)((context) => (b2) =>
                {
                    b2.OpenElement(0, "div");
                    b2.AddAttribute(0, "class", ValidationClasses.ForGridCell($"{collectionFieldName}_{context.VisibleIndex}_{column.FieldName}"));
                    b2.AddContent(0, context.DisplayText);
                    b2.CloseElement();
                }));
            }

            b.CloseComponent();
        }
    }
}
