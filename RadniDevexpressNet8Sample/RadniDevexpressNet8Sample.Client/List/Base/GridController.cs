using System.Dynamic;
using CommonBlazor.Extensions;
using CommonBlazor.Infrastructure;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components.Web;

namespace CommonBlazor.UI.List
{
    public class GridController<T> : IDisposable where T : class
    {
        public virtual DxGrid? Grid

        {
            get => State.Grid;
            set
            {
                State.Grid = value;
                if (value != null)
                {
                    OnGridInitialized();
                }
            }
        }

        private ListState _state;

        protected ListState State
        {
            get => _state;
            set
            {
                _state = value;
                _gridContextMenu.State = value;
            }
        }

        public DxContextMenu? ContextMenu
        {
            get => _contextMenu;
            set
            {
                _contextMenu = value;
                _gridContextMenu.ContextMenu = value;
            }
        }


        protected GridContextMenu _gridContextMenu = new GridContextMenu();

        public IReadOnlyList<object> SelectedDataItemsInternal { get => State.SelectedDataItems; set => State.SelectedDataItems = value; }

        public int SelectedRowCount => SelectedDataItemsInternal.Count();

        public List<T> SelectedDataItems { get => GetSelectedDataItems<T>(); set => State.SelectedDataItems = value; }

        internal object? SelectedDataItemInternal { get => State.SelectedDataItem; set => State.SelectedDataItem = value; }

        public T? SelectedDataItem { get => GetSelectedDataItem<T>(); set => State.SelectedDataItem = value; }

        public event Action? StateHasChanged;
        
        public event Action<T?>? SelectedDataItemChanged;
        public event Action<SelectedDataItemsChangedEventArgs<T>>? SelectedDataItemsChanged;

        public bool IsInitialized { get => State.IsInitialized; }

        public int SelectedPageIndex { get => State.SelectedPageIndex; set => State.SelectedPageIndex = value; }

        public bool IsRowEditing => State.IsRowEditing;

        public event Action<T>? DataItemChoosed;

        private GridSelectionMode _selectionMode;

        public GridSelectionMode SelectionMode
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;
                SelectionModeChanged?.Invoke();
            }
        }

        public event Action SelectionModeChanged;

        private DxContextMenu? _contextMenu;
       // private ExceptionHandler _exceptionHandler;

        public GridController()
        {
            State = new ListState();
           // _exceptionHandler = ServiceResolver.Resolve<ExceptionHandler>();
        }

        public virtual Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return Task.CompletedTask;

            //if (CommandManager != null)
            //{
            //    var copyCommand = new CopyCommand(State);
            //    var copyRowCommand = new CopyRowCommand(State);

            //    CommandManager
            //        .Add(UICommandBuilder.Create(UICommandFactory.Copy())
            //            .BeginGroup()
            //            .Execute(async (cancellationToken) => await copyCommand.ExecuteAsync(cancellationToken))
            //            .CanExecute(() => true)
            //            .Build())
            //        .Add(UICommandBuilder.Create(UICommandFactory.CopyRow())
            //            .Execute(async (cancellationToken) => await copyRowCommand.ExecuteAsync(cancellationToken))
            //            .CanExecute(() => true)
            //            .Build())
            //        .Update();
            //}

            //State.IsInitialized = true;

            //// update toolbar after grid initialized, because button state can be depends of IsInitialized property
            //if (CommandManager != null)
            //    CommandManager.Update();

            return Task.CompletedTask;
        }

        protected void RaiseStateHasChanged()
        {
            StateHasChanged?.Invoke();
        }

        public virtual TData? GetSelectedDataItem<TData>() where TData : class
        {
            return SelectedDataItemInternal?.CastTo<TData>();
        }

        public virtual List<TData> GetSelectedDataItems<TData>() where TData : class
        {
            return SelectedDataItemsInternal.Select(x => x.CastTo<TData>()).ToList();
        }

        public virtual TData? GetDataItem<TData>(int visibleIndex) where TData : class
        {
            return Grid?.GetDataItem(visibleIndex)?.CastTo<TData>();
        }

        public virtual object? GetSelectedValue(string field)
        {
            if (SelectedDataItemInternal == null || string.IsNullOrEmpty(field))
                return null;

            if (typeof(T) == typeof(ExpandoObject))
            {
                var obj = (ExpandoObject)Convert.ChangeType(SelectedDataItemInternal, typeof(ExpandoObject));
                obj.TryGetValue(field.ToLowerCaseLikeJsonSerializerOrConvertNullToEmptyString(), out var key);
                return key;
            }
            else
            {
                var property = typeof(T).GetProperty(field, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                if (property != null)
                    return property.GetValue(SelectedDataItemInternal);
                else
                    return null;
            }
        }

        public void OnCustomizeElement(GridCustomizeElementEventArgs e)
        {
            _gridContextMenu.AddContextMenuCustomizeElement(e);

            CustomizeElement?.Invoke(e);
        }

        public Action<GridCustomizeElementEventArgs> CustomizeElement;

        public void OnUnboundColumnData(GridUnboundColumnDataEventArgs eventArgs)
        {

        }

        public void OnPageSizeChanged(int pageSize)
        {
        }

        public void OnSelectedDataItemChanged(object newSelection)
        {
            State.SelectedDataItem = newSelection;
            //CommandManager?.Update();

            SelectedDataItemChanged?.Invoke(GetSelectedDataItem<T>());
        }

        public async void OnSelectedDataItemsChanged(IReadOnlyList<object> newSelection)
        {
            try
            {
                var oldSelection = State.SelectedDataItems.Select(x => x.CastTo<T>()).ToList();

                State.SelectedDataItems = newSelection;

                //CommandManager?.Update();

                var newConvertedSelection = GetSelectedDataItems<T>();

                var e = new SelectedDataItemsChangedEventArgs<T>
                {
                    OldSelection = oldSelection,
                    NewSelection = newConvertedSelection,
                    DataItemsSelected = newConvertedSelection.Except(oldSelection).ToList(),
                    DataItemsDeselected = oldSelection.Except(newConvertedSelection).ToList(),
                };

                SelectedDataItemsChanged?.Invoke(e);
            }
            catch (Exception ex)
            {
                //await _exceptionHandler.HandleAsync(ex, true);
                throw;
            }
        }

        public virtual void ReloadData(bool refreshFromServer = true)
        {
            if (Grid == null) return;
            ClearSelectionInternal();
            Grid.Reload();
            //CommandManager?.Update();
        }

        public void ClearSelection()
        {
            if (State.SelectedDataItem != null || State.SelectedDataItems.Count > 0)
            {
                State.SelectedDataItems = new List<object>();
                State.SelectedDataItem = null;
                StateHasChanged?.Invoke();
            }
        }

        protected void ClearSelectionInternal()
        {
            if (State.SelectedDataItem != null || State.SelectedDataItems.Count > 0)
            {
                State.SelectedDataItems = new List<object>();
                State.SelectedDataItem = null;
            }
        }

        public void OnRowClick(GridRowClickEventArgs e)
        {
            // OnRowClick is called before SelectedDataItemChanged

            DataItemChoosed?.Invoke((T)e.Grid.GetDataItem(e.VisibleIndex));
        }

        public virtual void Dispose()
        {
        }

        protected virtual void OnGridInitialized()
        {
        }

        public virtual void SelectDataItem(object dataItem)
        {
            if (Grid == null)
                throw new InvalidOperationException("Grid is not initialized");

            Grid.SelectDataItem(dataItem);
        }

        public virtual void SelectDataItems(IEnumerable<object> dataItem)
        {
            if (Grid == null)
                throw new InvalidOperationException("Grid is not initialized");

            Grid.SelectDataItems(dataItem);
        }

        public virtual void DeselectDataItem(object dataItem)
        {
            if (Grid == null)
                throw new InvalidOperationException("Grid is not initialized");

            Grid.DeselectDataItem(dataItem);
        }

        public virtual void DeselectDataItems(IEnumerable<object> dataItems)
        {
            if (Grid == null)
                throw new InvalidOperationException("Grid is not initialized");

            Grid.DeselectDataItems(dataItems);
        }

        public void OnEditStart(GridEditStartEventArgs e)
        {
            State.IsRowEditing = true;

            //CommandManager?.Update();

            StateHasChanged?.Invoke();
        }

        public void OnModelEditSaving(GridEditModelSavingEventArgs e)
        {
            State.IsRowEditing = false;

            //CommandManager?.Update();

            StateHasChanged?.Invoke();
        }

        public void OnEditCanceling(GridEditCancelingEventArgs e)
        {
            State.IsRowEditing = false;

            //CommandManager?.Update();

            StateHasChanged?.Invoke();
        }
    }
}
