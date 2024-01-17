using System;

namespace CommonBlazor.Infrastructure
{
    public interface IHttpClientDelegatingHandlerProvider
    {
        public Type GetHandlerType(string httpClientName);
    }
}
