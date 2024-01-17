namespace CommonBlazor.UI.List
{
    public class RemoveEntityMessage
    {
        public string EntityName { get; set; }

        public object EntityId { get; set; }

        public RemoveEntityMessage(string entityName, object entityId)
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
