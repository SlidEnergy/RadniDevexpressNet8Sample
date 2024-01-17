namespace CommonBlazor.UI.List
{
    public class ReloadGridMessage
    {
        public string EntityName { get; set; }

        public ReloadGridMessage(string entityName)
        {
            EntityName = entityName;
        }
    }
}
