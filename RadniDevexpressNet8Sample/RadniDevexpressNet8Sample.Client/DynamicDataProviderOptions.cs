namespace CommonBlazor.DynamicData
{
    public class DynamicDataProviderOptions
    {
        public string? BaseAddress { get; set; }
        public string? InitializationEndpoint { get; set; }
        public string? SaveColumnsEndpoint { get; set; }
        public string? QueryEndpoint { get; set; }
        public string? GetCountEndpoint { get; set; }
        public string? AvailableColumnsEndpoint { get; set; }
        public string? AvailableCollectionsEndpoint { get; set; }
        public string? ClientId { get; set; }
    }
}