namespace CommonBlazor.UI.List
{
    public class UpdateEntityMessage
    {
        public string EntityName { get; set; }

        public object EntityId { get; set; }

        public UpdateEntityMessage(string entityName, object entityId)
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
