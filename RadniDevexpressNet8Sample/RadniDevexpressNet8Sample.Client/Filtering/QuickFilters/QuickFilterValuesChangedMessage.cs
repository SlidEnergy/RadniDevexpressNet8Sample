using Common.DataAccess.Filtering;

namespace CommonBlazor.UI.Filtering.QuickFilters
{
    public class QuickFilterValuesChangedMessage
    {
        public IEnumerable<FilterCriteria>? QuickFilters { get; set; }
    }
}
