using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<bool> SetIdentityAsync(
            string username, string password, CancellationToken token = default)
        {
            RemoveIdentity();
            if (await client.GetStringAsync($"/api/State?usr={username}&pwd={password}") == "Authenticated")
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Yuko {username} {password}");
                state.TriggerStateChange();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
