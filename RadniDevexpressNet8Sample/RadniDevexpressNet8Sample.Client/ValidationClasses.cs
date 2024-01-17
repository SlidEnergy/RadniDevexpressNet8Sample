namespace CommonBlazor.UI.Components.Validation
{
    public static class ValidationClasses
    {
        public static string ForEditor(string fieldName)
        {
            return $"val-editor val-field-{fieldName}";
        }

        public static string ForCard(string fieldName)
        {
            return $"val-card val-field-{fieldName}";
        }

        public static string ForGridCell(string fieldName)
        {
            return $"val-grid-cell val-input val-field-{fieldName}";
        }
    }
}
