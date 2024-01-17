using Common.DataAccess.Sorting;

namespace CommonBlazor.DynamicData.Models
{
    public class ConfigurationModel
    {
        public List<GenericColumnSettings> Columns { get; set; }
        public IEnumerable<SortBySettings> SortBy { get; set; }

        public Type BaseType { get; set; }
        public string EntityName { get; set; }
    }
}
