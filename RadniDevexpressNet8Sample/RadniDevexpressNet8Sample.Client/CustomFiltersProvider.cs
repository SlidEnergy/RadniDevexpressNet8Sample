using System.Text.Json;
using Microsoft.Extensions.Options;
using Common.DataAccess.Filtering;
using CommonBlazor.Infrastructure;
using CommonBlazor.DynamicData.Abstractions;

namespace CommonBlazor.DynamicData.Filtering
{
    public class CustomFiltersProvider : ApiClient, ICustomFiltersProvider
    {
        private readonly CustomFiltersProviderOptions _options;

        public CustomFiltersProvider(
            HttpClientManager httpClientManager,
            IOptions<JsonSerializerOptions> serializerOptionsAccessor,
            IOptions<CustomFiltersProviderOptions> filtersProviderOptionsAccessor)
            : base(httpClientManager.GetClient("MyApplicationApi"), serializerOptionsAccessor.Value)
        {
            _options = filtersProviderOptionsAccessor.Value;
        }

        public async Task<List<FilterInfo>> GetAllAsync(string? key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = _options.GetAll;

            if (!string.IsNullOrEmpty(key))
                url += $"?key={key}";

            var response = await GetApiAsync<List<CustomFilterDto>>(url, cancellationToken);

            //if (!response.IsSuccessfull)
            //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApiException(error));

            return response.Result
                .Select(x => new FilterInfo(x) { CriteriaCollection  = x.FilterCriteries?.ToDictionary(x => x.Key, x => FilterCriteria.Parse(x.Value)) })
                .ToList();
        }

        public async Task<FilterInfo> GetDefaultFilterAsync(string? key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = _options.GetAll + "?";

            if (!string.IsNullOrEmpty(key))
                url += $"key={key}&";

            url += $"getdefaultfilters=true";

            var response = await GetApiAsync<List<CustomFilterDto>>(url, cancellationToken);

            //if (!response.IsSuccessfull)
            //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApiException(error));

            return response.Result
                .Select(x => new FilterInfo(x) { CriteriaCollection = x.FilterCriteries?.ToDictionary(x => x.Key, x => FilterCriteria.Parse(x.Value)) })
                .FirstOrDefault();
        }

        public async Task<List<FilterInfo>> GetPrefiltersAsync(string? key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var url = _options.GetAll + "?";

            if (!string.IsNullOrEmpty(key))
                url += $"key={key}&";

            url += $"getprefilters=true";

            var response = await GetApiAsync<List<CustomFilterDto>>(url, cancellationToken);

            //if (!response.IsSuccessfull)
            //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApiException(error));

            return response.Result
                .Select(x => new FilterInfo(x) { CriteriaCollection = x.FilterCriteries?.ToDictionary(x => x.Key, x => FilterCriteria.Parse(x.Value)) })
                .ToList();
        }

        public async Task<FilterInfo> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await GetApiAsync<CustomFilterDto>($"{_options.GetById}/{id}", cancellationToken);

            //if (!response.IsSuccessfull)
            //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApiException(error));

            return new FilterInfo(response.Result) { CriteriaCollection = response.Result.FilterCriteries?.ToDictionary(x => x.Key, x => FilterCriteria.Parse(x.Value)) };

        }

        //public async Task<FilterInfo> SaveAsync(string key, FilterInfo filter, string section, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    filter.Model.FilterCriteries = filter.CriteriaCollection.Where(x => x.Value is not null).ToDictionary(x => x.Key, x => x.Value.ToString());
        //    filter.Model.Section = section;

        //    var request = new SaveFilterRequest()
        //    {
        //        Key = key,
        //        CustomFilter = filter.Model,
        //    };
        //    var response = await PostApiAsync<CustomFilterDto>(_options.Save, request, cancellationToken);

        //    //if (!response.IsSuccessfull)
        //    //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApiException(error));

        //    return new FilterInfo(response.Result) { CriteriaCollection = response.Result.FilterCriteries?.ToDictionary(x => x.Key, x => FilterCriteria.Parse(x.Value)) };
        //}

        public async Task DeleteAsync(string key, int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await DeleteApiAsync<int>(_options.Delete + "/" + id, cancellationToken);
        }
    }
}
