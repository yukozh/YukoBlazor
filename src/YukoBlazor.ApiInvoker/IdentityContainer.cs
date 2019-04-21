using System;
using System.Net.Http;

namespace YukoBlazor.ApiInvoker
{
    public class IdentityContainer : IDisposable
    {
        public event Action OnIdentityChanged;
        private HttpClient client;
        private AppState state;

        public bool IsAuthenticated => client.DefaultRequestHeaders.Contains("Authorization");

        public IdentityContainer(HttpClient client, AppState state)
        {
            this.client = client;
            this.state = state;
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
                state.TriggerStateChange();
            }
        }

        public void SetIdentity(string username, string password)
        {
            RemoveIdentity();
            client.DefaultRequestHeaders.Add("Authorization", $"Yuko {username} {password}");
            state.TriggerStateChange();
        }
    }
}
