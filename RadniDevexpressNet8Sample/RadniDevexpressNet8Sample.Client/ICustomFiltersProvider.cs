using CommonBlazor.DynamicData.Filtering;

namespace CommonBlazor.DynamicData.Abstractions
{
    public interface ICustomFiltersProvider
    {
        Task<List<FilterInfo>> GetAllAsync(string? key, CancellationToken cancellationToken = default);
        Task<FilterInfo?> GetDefaultFilterAsync(string? key, CancellationToken cancellationToken = default);
        Task<List<FilterInfo>> GetPrefiltersAsync(string? key, CancellationToken cancellationToken = default);
        Task<FilterInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        //Task<FilterInfo> SaveAsync(string key, FilterInfo filter, string section, CancellationToken cancellationToken = default);

        Task DeleteAsync(string key, int id, CancellationToken cancellationToken = default);
    }
}