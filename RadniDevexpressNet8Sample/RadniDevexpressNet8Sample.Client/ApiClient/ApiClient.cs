using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Infrastructure.Exceptions;
using CommonBlazor.Models;

namespace CommonBlazor.Infrastructure
{
    public abstract class ApiClient
    {
        protected readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        protected ApiClient(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
        {
            _httpClient = httpClient;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        #region ApiResponse

        public async Task<ApiResponse<TResult>> GetApiAsync<TResult>(string uri, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.GetAsync(uri, cancellationToken);
            var stringContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.IsSuccessStatusCode == false)
            {
                await ParseErrorContent(httpResponse, "get", cancellationToken);
                //var errorModel = JsonSerializer.Deserialize<ErrorResponseModel>(stringContent, _jsonSerializerOptions);
                //_toastMessageService.ShowError(errorModel);

                //return new ApiResponse<TResult>(httpResponse.StatusCode, errorModel.Message);
            }

            var result = JsonSerializer.Deserialize<TResult>(stringContent, _jsonSerializerOptions);
            return ApiResponse.From(httpResponse.StatusCode, result);
        }

        public async Task<ApiResponse<TResult>> PostApiAsync<TResult>(string uri, object content, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(uri, content, cancellationToken);
            var stringContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.IsSuccessStatusCode == false)
            {
                await ParseErrorContent(httpResponse, "post", cancellationToken);
                //var errorModel = JsonSerializer.Deserialize<ErrorResponseModel>(stringContent, _jsonSerializerOptions);
                //_toastMessageService.ShowError(errorModel);

                //return new ApiResponse<TResult>(httpResponse.StatusCode, errorModel.Message);
            }

            var result = JsonSerializer.Deserialize<TResult>(stringContent, _jsonSerializerOptions);
            return ApiResponse.From(httpResponse.StatusCode, result);
        }

        public async Task<ApiResponse<string>> PostApiAsync(string uri, object content, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(uri, content, cancellationToken);
            var stringContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.IsSuccessStatusCode == false)
            {
                await ParseErrorContent(httpResponse, "post", cancellationToken);
                //var errorModel = JsonSerializer.Deserialize<ErrorResponseModel>(stringContent, _jsonSerializerOptions);
                //_toastMessageService.ShowError(errorModel);

                //return new ApiResponse<string>(httpResponse.StatusCode, errorModel.Message);
            }

            return ApiResponse.From(httpResponse.StatusCode, stringContent);
        }

        public async Task<ApiResponse<TResult>> DeleteApiAsync<TResult>(string uri, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.DeleteAsync(uri, cancellationToken);
            var stringContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

            if (httpResponse.IsSuccessStatusCode == false)
            {
                await ParseErrorContent(httpResponse, "delete", cancellationToken);
                //var errorModel = JsonSerializer.Deserialize<ErrorResponseModel>(stringContent, _jsonSerializerOptions);
                //_toastMessageService.ShowError(errorModel);

                //return new ApiResponse<TResult>(httpResponse.StatusCode, errorModel.Message);
            }

            var result = JsonSerializer.Deserialize<TResult>(stringContent, _jsonSerializerOptions);
            return ApiResponse.From(httpResponse.StatusCode, result);
        }

        #endregion

        #region Http methods

        public async Task<TResult> GetAsync<TResult>(string uri, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.GetAsync(uri, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                await ParseErrorContent(httpResponse, "get", cancellationToken);
            }

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            if (httpResponse.Content is null)
                return default;

            return await Deserialize<TResult>(httpResponse, cancellationToken);
        }

        public async Task<Stream> GetStreamAsync(string uri, CancellationToken cancellationToken = default)
        {
            var httpResponse = await _httpClient.GetAsync(uri, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                await ParseErrorContent(httpResponse, "get", cancellationToken);
            }

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            if (httpResponse.Content is null)
                return default;

            var content = await httpResponse.Content.ReadAsStreamAsync();

            return content;
        }


        public async Task<TResponse> PostAsync<TResponse>(string apiUrl, TResponse postObject, CancellationToken cancellationToken)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(apiUrl, postObject, cancellationToken).ConfigureAwait(false);
            if (!httpResponse.IsSuccessStatusCode)
            {
                await ParseErrorContent(httpResponse, "post", cancellationToken);
            }
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            if (httpResponse.Content is null)
                return default;

            return await Deserialize<TResponse>(httpResponse, cancellationToken);
        }

        public async Task<TResponse> PostAsync<TResponse>(string apiUrl, object postObject, CancellationToken cancellationToken)
        {
            var httpResponse = await _httpClient.PostAsJsonAsync(apiUrl, postObject, cancellationToken).ConfigureAwait(false);
            if (!httpResponse.IsSuccessStatusCode)
            {
                await ParseErrorContent(httpResponse, "post", cancellationToken);
            }

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            if (httpResponse.Content is null)
                return default;

            return await Deserialize<TResponse>(httpResponse, cancellationToken);
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string apiUrl, object payload, CancellationToken cancellationToken)
        {
            var httpResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, apiUrl) { Content = Serialize(payload) }).ConfigureAwait(false);
            if (!httpResponse.IsSuccessStatusCode)
            {
                await ParseErrorContent(httpResponse, "delete", cancellationToken);
            }

            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                return default;

            if (httpResponse.Content is null)
                return default;

            return await Deserialize<TResponse>(httpResponse, cancellationToken);
        }

        #endregion

        #region Shared methods

        private StringContent Serialize(object @object)
        {
            var payload = JsonSerializer.Serialize(@object);

            return new StringContent(payload, Encoding.UTF8, "application/json");
        }

        private async Task<TResponse> Deserialize<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (String.IsNullOrEmpty(content))
                return default(TResponse);

            if (typeof(TResponse).Equals(typeof(string)))
                return (TResponse)(object)content;

            return JsonSerializer.Deserialize<TResponse>(content, _jsonSerializerOptions);
        }


        private async Task ParseErrorContent(HttpResponseMessage httpResponse, string methodType, CancellationToken cancellationToken)
        {
            var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                if (!String.IsNullOrEmpty(content))
                {
                    var res = JsonSerializer.Deserialize<ErrorResponseModel>(content, _jsonSerializerOptions);
                    throw new ApplicationCustomException(res.Message, res.InternalCode);
                }
                else
                {
                    throw new ApplicationException($"Failed to {methodType} data. {httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(content))
                {
                    var res = JsonSerializer.Deserialize<HttpErrorExceptionModel>(content, _jsonSerializerOptions);
                    var errorString = string.Join("", res.Errors.Id);
                    throw new ApplicationException($"Failed to {methodType} data. {httpResponse.StatusCode} {errorString}");
                }
                else
                {
                    throw new ApplicationException($"Failed to {methodType} data. {httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                }
            }
        }

        public string GetUrl()
        {
            if (_httpClient == null)
                return String.Empty;

            return _httpClient.BaseAddress.ToString();
        }

        public string GetJwtToken()
        {
            if (_httpClient == null || _httpClient.DefaultRequestHeaders == null
                || _httpClient.DefaultRequestHeaders.Authorization == null)
                return String.Empty;

            var authoration = _httpClient.DefaultRequestHeaders.Authorization;

            return authoration.Parameter;
        }

        #endregion

    }
}
