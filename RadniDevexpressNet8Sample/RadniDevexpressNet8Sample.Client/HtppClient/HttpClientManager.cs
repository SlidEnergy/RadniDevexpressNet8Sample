using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CommonBlazor.Infrastructure
{
    public class HttpClientManager
    {
        private readonly Dictionary<string, HttpClient> _httpClientStore = new();

        public HttpClientManager(IServiceProvider serviceProvider, IEnumerable<ApiHttpClientModel> apiHttpClientModels, IOptions<HttpClientOptions> optionsAcessor)
        {
            var apiCollection = optionsAcessor.Value.ApiCollection.ToDictionary(x => x.Name, x => x.BaseAddress);
            var httpClientModels = apiHttpClientModels.ToDictionary(x => x.Name, x => x);
            if (apiCollection.Count != httpClientModels.Count)
                throw new InvalidOperationException("Invalid http client configuration.");

            foreach (var api in apiCollection)
            {
                var model = httpClientModels[api.Key];
                var httpClient = model.HandlerDelegateType is null
                    ? new HttpClient()
                    : new HttpClient((HttpMessageHandler)serviceProvider.GetRequiredService(model.HandlerDelegateType));

                httpClient.BaseAddress = new Uri(api.Value);
                _httpClientStore.Add(api.Key, httpClient);
            }
        }

        public HttpClient GetClient(string name)
        {
            return _httpClientStore[name];
        }
    }
}
