namespace CommonBlazor.DynamicData
{
    public class DynamicEntityContext : EntityContextBase
    {
        public string Key { get; set; }

        public DynamicEntityContext(string key)
        {
            Key = key;
        }
    }
}
