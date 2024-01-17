using CommonBlazor.DynamicData.Filtering;

namespace CommonBlazor.UI.Filtering
{
    public interface IFilteringService
    {
        Task<FilterInfo?> GetDefaultFilterAsync(string key, CancellationToken token = default);
    }
}