namespace CommonBlazor.DynamicData.Abstractions
{
    public class SaveColumnDto
    {
        public string FullPropertyName { get; set; }

        public string DisplayName { get; set; }

        public string DisplayFormat { get; set; }

        public int Order { get; set; }

        public int SortOrder { get; set; }

        public bool SortDesceding { get; set; }

        public decimal? ColumnWidth { get; set; }
    }
}
