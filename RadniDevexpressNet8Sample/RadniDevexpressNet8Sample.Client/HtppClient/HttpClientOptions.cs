using System.Collections.Generic;

namespace CommonBlazor.Infrastructure
{
    public class HttpClientOptions
    {
        public class ApiOptions
        {
            public string Name { get; set; }
            public string BaseAddress { get; set; }
        }

        public IEnumerable<ApiOptions> ApiCollection { get; set; }
    }
}
