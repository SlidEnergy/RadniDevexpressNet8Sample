using CommonBlazor.DynamicData.Models;
using System.Text.Json.Serialization;

namespace CommonBlazor.DynamicData
{
    public class EntityContextBase
    {
        public string? EntityName { get; set; }

        public string KeyField => Properties?.Take(1).Select(x => x.FullPropertyName).FirstOrDefault() ?? "Id";
        public Type KeyFieldType => Properties?.Take(1).Select(x => x.ParsedType).FirstOrDefault() ?? throw new Exception("Can't define key field type.");

        public string ScopeId { get; set; }

        public List<GenericColumnSettings>? Properties { get; set; }

        [JsonIgnore]
        public IEnumerable<GenericColumnSettings>? VisibleProperties => Properties?.Where(x => x.Visible);

        private bool _isInitialized;

        [JsonIgnore]
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            set
            {
                _isInitialized = value;
                Initialized?.Invoke();
            }
        }

        public event Action? Initialized;

        public EntityContextBase()
        {
            ScopeId = Guid.NewGuid().ToString();
        }
    }
}
