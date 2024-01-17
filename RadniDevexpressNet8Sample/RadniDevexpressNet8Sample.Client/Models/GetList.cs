namespace CommonBlazor.DynamicData.Models
{
    public class GetListRequest
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

        public bool Distinct { get; set; }
    }

    public class ListSortModel
    {
        public string? ColumnName { get; set; }
        public bool Ascending { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != typeof(ListSortModel))
                return false;

            return ((ListSortModel)obj).ColumnName.Equals(ColumnName) && ((ListSortModel)obj).Ascending.Equals(Ascending);
        }
    }
}
