using DevExpress.Blazor;
using Common.DataAccess.Filtering;
using Common.DataAccess.Sorting;

namespace CommonBlazor.UI.List
{
    public abstract class GridCustomDataSourceBase : GridCustomDataSource
    {
        public FilterCriteria? Filter { get; set; }

        public IEnumerable<SortBySettings>? SortBy { get; set; }

        public Dictionary<string, FilterCriteria>? CollectionFilters { get; set; }

        public IEnumerable<FilterCriteria>? QuickFilters { get; set; }

        public virtual event Action? AllDataRefreshed;

        public bool DataIsLoaded { get; set; }
    }
}
