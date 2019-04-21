using System;
using System.Net.Http;

namespace YukoBlazor.ApiInvoker
{
    public partial class ApiClient : IDisposable
    {
        private HttpClient client;
        private IdentityContainer container;

        public ApiClient(HttpClient client, IdentityContainer container)
        {
            this.client = client;
            this.container = container;
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
