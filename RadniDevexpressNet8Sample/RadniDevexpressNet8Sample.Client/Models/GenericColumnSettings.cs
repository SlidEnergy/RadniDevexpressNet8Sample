using System.Text.Json.Serialization;
using static DevExpress.Data.Helpers.LohPooled;

namespace CommonBlazor.DynamicData.Models
{
    public class GenericColumnSettings
    {
        public int Order { get; set; }
        public string EntityName { get; set; }
        public string PropertyName { get; set; }
        public string FullPropertyName { get; set; }
        public string Type { get; set; }

        private Type _parsedType;

        [JsonIgnore]
        public Type ParsedType
        {
            get
            {
                if (_parsedType == null)
                {
                    _parsedType = TypeUtils.GetType(Type);

                    if (_parsedType == null)
                        throw new DynamicDataParseException("Can't parse serialized type of generic column settings. Type: " + Type);
                }

                return _parsedType;
            }
            set => _parsedType = value;
        }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsNavigationProperty { get; set; }
        public string DisplayName { get; set; }
        public string DisplayFormat { get; set; }
        public decimal? ColumnWidth { get; set; }
        public bool Visible { get; set; }
        public bool AddToGrid { get; set; }

        public int SortOrder { get; set; }

        public bool SortDesceding { get; set; }

        public string NavigationPropertyKey { get; set; }
        public string NavigationPropertyPath { get; set; }

        public GenericColumnSettings Clone()
        {
            return (GenericColumnSettings)this.MemberwiseClone();
        }

        public override bool Equals(object? obj)
        {
            if(obj == null) return false;

            if(obj.GetType() != typeof(GenericColumnSettings))
                return false;

            return ((GenericColumnSettings)obj).FullPropertyName == FullPropertyName;
        }
    }

    public class GenericColumnSettingsComparer : IEqualityComparer<GenericColumnSettings>
    {
        bool IEqualityComparer<GenericColumnSettings>.Equals(GenericColumnSettings x, GenericColumnSettings y)
        {
            return x.FullPropertyName == y.FullPropertyName;
        }

        int IEqualityComparer<GenericColumnSettings>.GetHashCode(GenericColumnSettings obj)
        {
            return obj.FullPropertyName.GetHashCode();
        }
    }
}
