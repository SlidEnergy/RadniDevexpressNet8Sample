using System.ComponentModel;
using Newtonsoft.Json;
using Common.DataAccess.Filtering;
using CommonBlazor.Infrastructure;
using CommonBlazor.UI.Filtering.Prefilters;
using CommonBlazor.UI.Filtering.QuickFilters;
using CommonBlazor.DynamicData.Filtering;
using CommonBlazor.DynamicData.Abstractions;
using CommonBlazor.DynamicData;

namespace CommonBlazor.UI.Filtering
{
    public class FilteringState
    {
        private IMessenger? _messenger;

        public IMessenger Messenger { get => _messenger ?? throw ThrowHelper.PropertyIsNull(); set => _messenger = value; }

        public bool EditPopupVisible
        {
            get => _editPopupVisible; set
            {
                if (_editPopupVisible == value)
                    return;

                _editPopupVisible = value;

                EditPopupVisibleChanged?.Invoke();
            }
        }

        public event Action? EditPopupVisibleChanged;

        private bool _filtersPopupVisible;

        public bool PrefiltersPopupVisible
        {
            get => _filtersPopupVisible;
            set
            {
                if (_filtersPopupVisible == value)
                    return;

                _filtersPopupVisible = value;
                PrefiltersPopupVisibleChanged?.Invoke();
            }
        }

        public event Action? PrefiltersPopupVisibleChanged;

        private bool _quickFiltersPopupVisible;

        public bool QuickFiltersPopupVisible
        {
            get => _quickFiltersPopupVisible;
            set
            {
                _quickFiltersPopupVisible = value;
                QuickFiltersPopupVisibleChanged?.Invoke();
            }
        }

        public event Action? QuickFiltersPopupVisibleChanged;

        public FilterInfo? EditFilter { get; set; }

        public FilterInfo? SelectedFilter
        {
            get
            {
                if (SelectedFilters == null || SelectedFilters.Count() < 1)
                    return null;

                return (FilterInfo)SelectedFilters.First();
            }
            set
            {
                if (value == null)
                    SelectedFilters = null;
                else
                    SelectedFilters = new List<FilterInfo>()
                    {
                        value
                    };
            }
        }

        public IReadOnlyList<object>? SelectedFilters { get; set; }

        public bool HasDefaultFilter => Filters.Any(x => x.DefaultFilter == true);

        public BindingList<FilterInfo> Filters { get; set; } = new BindingList<FilterInfo>();

        public FilterInfo? CurrentFilter { get; set; }

        public event Action? CurrentFilterChanged;

        public bool IsNewFilter = false;
        private bool _editPopupVisible;

        public List<QuickFilterInfo>? QuickFilters { get; set; }

        public FilteringState(DynamicEntityContext entityContext)
        {
            Messenger = ServiceResolver.Resolve<IMessenger>(entityContext.ScopeId);
        }

        public void ShowNewPopup()
        {
            EditFilter = new FilterInfo(new CustomFilterDto() { IsNew = true });

            EditPopupVisible = true;

            IsNewFilter = true;
        }
        public void ShowEditPopup()
        {
            if (SelectedFilter == null)
                return;

            //var filter = SelectedFilter;

            //EditFilter = new FilterInfo(new CustomFilterDto()
            //{
            //    Id = filter.Id,
            //    Name = filter.Name,
            //    FilterCriteries = filter.Model.FilterCriteries,
            //    Prefilter = filter.Model.Prefilter,
            //    DefaultFilter = filter.Model.DefaultFilter,
            //    DisplayName = filter.DisplayName,
            //});
            //EditFilter.DevextremeCriteria = filter.DevextremeCriteria;
            //EditFilter.Criteria = filter.Criteria;

            //EditFilter.CriteriaCollection = filter.CriteriaCollection?.ToDictionary(x => x.Key, x=> x.Value);
            //EditFilter.DevextremeCriteriaCollection = filter.DevextremeCriteriaCollection?.ToDictionary(x => x.Key, x => x.Value);

            EditPopupVisible = true;

            IsNewFilter = false;
        }

        public void EndEditFilter()
        {
            if (EditFilter == null)
                throw ThrowHelper.PropertyIsNull(nameof(EditFilter));

            if (IsNewFilter)
            {
                Filters.Add(EditFilter);
            }
            else
            {
                if (SelectedFilter == null)
                    throw ThrowHelper.PropertyIsNull(nameof(SelectedFilter));

                SelectedFilter.Name = EditFilter.Name;
                SelectedFilter.DisplayName = EditFilter.DisplayName;
                SelectedFilter.Prefilter = EditFilter.Prefilter;
                SelectedFilter.DefaultFilter = EditFilter.DefaultFilter;

                var oldCriteria = SelectedFilter.Criteria?.ToString();
                var newCriteria = EditFilter.Criteria?.ToString();

                if (string.Compare(oldCriteria, newCriteria, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    SelectedFilter.Criteria = EditFilter.Criteria;
                    SelectedFilter.DevextremeCriteria = EditFilter.DevextremeCriteria;
                    SendFilterChangedMessage(SelectedFilter);
                }

                var oldCriteriaCollection = JsonConvert.SerializeObject(SelectedFilter.CriteriaCollection?.Where(x => x.Value is not null).ToDictionary(x => x.Key, x => x.Value!.ToString()));
                var newCriteriaCollection = JsonConvert.SerializeObject(EditFilter.CriteriaCollection?.Where(x => x.Value is not null).ToDictionary(x => x.Key, x => x.Value!.ToString()));

                if (string.Compare(oldCriteriaCollection, newCriteriaCollection, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    SelectedFilter.CriteriaCollection = EditFilter.CriteriaCollection;
                    SelectedFilter.DevextremeCriteriaCollection = EditFilter.DevextremeCriteriaCollection;
                    SendFilterChangedMessage(SelectedFilter);
                }
            }

            EditFilter = null;
        }

        public void SendFilterChangedMessage(FilterInfo? filter)
        {
            Messenger.Send(new PrefilterChangedMessage() { CollectionFilters = filter?.CriteriaCollection.Where(x => x.Value is not null).ToDictionary(x => x.Key, x => x.Value!) });
        }

        public void SetCurrentFilter(FilterInfo? filter, bool sendMessage = true)
        {
            if (CurrentFilter == filter)
                return;

            CurrentFilter = filter;

            if(sendMessage)
                SendFilterChangedMessage(filter);

            CurrentFilterChanged?.Invoke();
        }

        public void SendQuickFiltersChangedMessage(IEnumerable<FilterCriteria>? filters)
        {
            Messenger.Send(new QuickFilterValuesChangedMessage() { QuickFilters = filters });
        }

        public void DeleteSelectedFilter()
        {
            if (SelectedFilter == null)
                return;

            if (CurrentFilter == SelectedFilter)
                SetCurrentFilter(null);

            Filters.Remove(SelectedFilter);

            SelectedFilter = null;
        }
    }
}
