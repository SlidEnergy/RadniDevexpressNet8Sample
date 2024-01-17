using System.Text.Json;
using Microsoft.Extensions.Options;
using CommonBlazor.Infrastructure;

namespace AndromedaBlazor.Api
{
    public class ApplicationApiClient : ApiClient
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ApplicationApiClient(
            HttpClientManager httpClientManager,
            IOptions<JsonSerializerOptions> serializerOptionsAccessor)
            : base(httpClientManager.GetClient("MyApplicationApi"), serializerOptionsAccessor.Value)
        {
            _jsonSerializerOptions = serializerOptionsAccessor.Value;
        }
    }
}
