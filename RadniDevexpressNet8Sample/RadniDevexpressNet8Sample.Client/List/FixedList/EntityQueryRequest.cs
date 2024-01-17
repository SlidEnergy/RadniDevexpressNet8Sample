namespace CommonBlazor.UI.List
{
    public class EntityQueryRequest
    {
        public string? Key { get; set; }
        public IEnumerable<string>? SelectColumns { get; set; }
        public IEnumerable<ListSortModel>? SortByColumns { get; set; }
        public IEnumerable<string>? GroupByColumns { get; set; }
        public string? FilterText { get; set; }
        public Dictionary<string, string>? CollectionFilters { get; set; }
        public string? QuickFilter { get; set; }
        public int SkipRows { get; set; }
        public int TakeRows { get; set; }
    }

    public class ListSortModel
    {
        public string? ColumnName { get; set; }
        public bool Ascending { get; set; }
    }
}
