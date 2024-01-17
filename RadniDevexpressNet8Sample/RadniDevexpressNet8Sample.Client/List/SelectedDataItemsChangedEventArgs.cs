namespace CommonBlazor.UI.List
{
    public class SelectedDataItemsChangedEventArgs<T> where T : class
    {
        public List<T> OldSelection { get; set; } = new List<T>();

        public List<T> NewSelection { get; set; } = new List<T>();

        public List<T> DataItemsSelected { get; set; } = new List<T>();

        public List<T> DataItemsDeselected { get; set; } = new List<T>();
    }
}