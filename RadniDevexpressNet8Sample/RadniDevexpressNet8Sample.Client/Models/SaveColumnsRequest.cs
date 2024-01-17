using CommonBlazor.DynamicData.Abstractions;

namespace CommonBlazor.DynamicData.Models
{
    public class SaveColumnsRequest
    {
        public string Key { get; set; } 

        public IEnumerable<SaveColumnDto> Columns { get; set; }
    }
}
