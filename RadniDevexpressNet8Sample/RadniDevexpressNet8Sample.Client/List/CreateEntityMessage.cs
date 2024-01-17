namespace CommonBlazor.UI.List
{
    public class CreateEntityMessage
    {
        public string EntityName { get; set; }

        public object EntityId { get; set; }

        public CreateEntityMessage(string entityName, object entityId)
        {
            EntityName = entityName;
            EntityId = entityId;
        }
    }
}
