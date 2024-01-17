namespace CommonBlazor.DynamicData.Abstractions
{
    public class CustomFilterDto
    {
        public int Id { get; set; }

        public bool IsNew { get; set; }
        public bool IsDirty { get; set; }
        public string TableName { get; set; }
        public string Section { get; set; }
        public int OrdinalNo { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, string> FilterCriteries { get; set; }
        public bool Prefilter { get; set; }
        public bool DefaultFilter { get; set; }
    }
}
