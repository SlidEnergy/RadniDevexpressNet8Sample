namespace CommonBlazor.DynamicData.Models
{
    public class GenericCollectionSettings
    {
        public string EntityName { get; set; }
        public string CollectionEntityName { get; set; }
        public string PropertyName { get; set; }
        public string FullPropertyName { get; set; }
        public string Type { get; set; }
        public string DisplayName { get; set; }
        public bool Visible { get; set; }
        public bool AddToGrid { get; set; }
        public string NavigationPropertyKey { get; set; }
        public string NavigationPropertyPath { get; set; }

        public GenericCollectionSettings Clone()
        {
            return (GenericCollectionSettings)this.MemberwiseClone();
        }
    }
}
