using System.Dynamic;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CommonBlazor.Infrastructure;
using CommonBlazor.DynamicData.Models;
using CommonBlazor.DynamicData.Abstractions;

namespace CommonBlazor.DynamicData
{
    public class DynamicDataProvider : ApiClient, IDynamicDataProvider
    {
        private readonly DynamicDataProviderOptions _options;

        public DynamicDataProvider(
            HttpClientManager httpClientManager,
            IOptions<JsonSerializerOptions> serializerOptionsAccessor,
            IOptions<DynamicDataProviderOptions> dynamicDataProviderOptionsAccessor)
            : base(httpClientManager.GetClient("MyApplicationApi"), serializerOptionsAccessor.Value)
        {
            _options = dynamicDataProviderOptionsAccessor.Value;
        }

        public async Task<IEnumerable<GenericColumnSettings>> GetAvailableColumnsAsync(string key, string? path, CancellationToken cancellationToken = default)
        {
            var response = await GetApiAsync<IEnumerable<GenericColumnSettings>>($"{_options.AvailableColumnsEndpoint}?key={key}&path={path}", cancellationToken);

            return response.Result;
        }

        public async Task<IEnumerable<GenericCollectionSettings>> GetAvailableCollectionsAsync(string key, string? path, CancellationToken cancellationToken = default)
        {
            var response = await GetApiAsync<IEnumerable<GenericCollectionSettings>>($"{_options.AvailableCollectionsEndpoint}?key={key}&path={path}", cancellationToken);

            return response.Result;
        }

        public Task<IList<ExpandoObject>> GetAllAsync(string key, IEnumerable<GenericColumnSettings> properties, IEnumerable<ListSortModel>? sortByColumns, CancellationToken cancellationToken = default)
        {
            return GetListAsync(key, properties, null, new Dictionary<string, string>(), null, sortByColumns, null, 0, 0, cancellationToken);
        }

        public async Task<IList<ExpandoObject>> GetListAsync(string key, IEnumerable<GenericColumnSettings> properties, string? gridFilter,
            Dictionary<string, string>? collectionFilters, string? quickFilter, IEnumerable<ListSortModel>? sortByColumns, IEnumerable<string>? groupByColumns, int skipRows, int takeRows,
            CancellationToken cancellationToken = default)
        {
            var request = new GetListRequest()
            {
                Key = key,
                SkipRows = skipRows,
                TakeRows = takeRows,
                FilterText = gridFilter,
                CollectionFilters = collectionFilters,
                QuickFilter = quickFilter,
                SortByColumns = sortByColumns,
                GroupByColumns = groupByColumns,
                SelectColumns = properties.Select(x => x.FullPropertyName).ToList()
            };

            var response = await PostApiAsync($"{_options.QueryEndpoint}", request, cancellationToken);

            //if (!response.IsSuccessfull)
            //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApplicationCustomException(error));

            var result = DynamicDataJsonParser.ParseList(response.Result, properties).ToList();

            return result;
        }

        public async Task<int> GetItemCountAsync(string key, IEnumerable<GenericColumnSettings> properties, string? gridFilter,
            Dictionary<string, string> collectionFilters, string? quickFilter, bool unique,
            CancellationToken cancellationToken = default)
        {
            var request = new GetListRequest()
            {
                Key = key,
                FilterText = gridFilter,
                CollectionFilters = collectionFilters,
                QuickFilter = quickFilter,
                Distinct = unique,
                SelectColumns = properties.Select(x => x.FullPropertyName).ToList()
            };

            var response = await PostApiAsync($"{_options.GetCountEndpoint}", request, cancellationToken);

            //if (!response.IsSuccessfull)
            //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApiException(error));

            var result = JsonConvert.DeserializeObject<int>(response.Result);

            return result;
        }

        public async Task<ConfigurationModel> GetConfigurationModelAsync(string key, CancellationToken cancellationToken = default)
        {
            var response = await GetApiAsync<ConfigurationModel>($"{_options.InitializationEndpoint}?clientId={_options.ClientId}&key={key}", cancellationToken);

            //if (!response.IsSuccessfull)
            //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApiException(error));

            return response.Result;
        }

        public async Task<IEnumerable<GenericCollectionSettings>> SaveColumns(string key, IEnumerable<GenericColumnSettings> columns, CancellationToken cancellationToken = default)
        {
            var requst = new SaveColumnsRequest()
            {
                Key = key,
                Columns = columns.Select(x => new SaveColumnDto()
                {
                    DisplayFormat = x.DisplayFormat,
                    DisplayName = x.DisplayName,
                    FullPropertyName = x.FullPropertyName,
                    Order = x.Order,
                    SortOrder = x.SortOrder,
                    SortDesceding = x.SortDesceding,
                    ColumnWidth = x.ColumnWidth
                })
            };

            var response = await PostApiAsync<IEnumerable<GenericCollectionSettings>>($"{_options.SaveColumnsEndpoint}", requst, cancellationToken);

            //if (!response.IsSuccessfull)
            //    response.ThrowException(System.Net.HttpStatusCode.OK, error => new ApiException(error));

            return response.Result;
        }
    }
}
