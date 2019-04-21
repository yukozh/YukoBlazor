using System;
using System.Net.Http;

namespace YukoBlazor.ApiInvoker
{
    public class IdentityContainer : IDisposable
    {
        private HttpClient client;

        public bool IsAuthenticated => client.DefaultRequestHeaders.Contains("Authorization");

        public IdentityContainer(HttpClient client)
        {
            this.client = client;
        }

        public void Dispose()
        {
            if (client != null)
            {
                RemoveIdentity();
            }
        }

        public void RemoveIdentity()
        {
            if (client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Remove("Authorization");
            }
        }

        public void SetIdentity(string username, string password)
        {
            RemoveIdentity();
            client.DefaultRequestHeaders.Add("Authorization", $"Yuko {username} {password}");
        }
    }
}
