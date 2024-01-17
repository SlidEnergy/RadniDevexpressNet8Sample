using System;

namespace CommonBlazor.Infrastructure
{
    public class ApiHttpClientModel
    {
        public string Name { get; }
        public Type HandlerDelegateType { get; }

        public ApiHttpClientModel(string name)//, IHttpClientDelegatingHandlerProvider httpClientDelegatingHandlerProvider)
        {
            Name = name;
            //HandlerDelegateType = httpClientDelegatingHandlerProvider.GetHandlerType(name);
        }
    }
}
