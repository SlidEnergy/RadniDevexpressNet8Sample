using Common.DataAccess.Filtering;

namespace CommonBlazor.UI.Filtering.Prefilters
{
    public class PrefilterChangedMessage
    {
        public FilterCriteria? Filter { get; set; }

        public Dictionary<string, FilterCriteria>? CollectionFilters { get; set; }
    }
}
