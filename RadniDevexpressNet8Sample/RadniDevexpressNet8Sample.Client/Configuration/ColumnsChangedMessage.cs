using Common.DataAccess.Filtering;
using CommonBlazor.DynamicData.Models;

namespace CommonBlazor.UI.Configuration
{
    public class ColumnsChangedMessage
    {
        public List<GenericColumnSettings>? Columns { get; set; }
    }
}
