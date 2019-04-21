using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using YukoBlazor.Shared;

namespace YukoBlazor.ApiInvoker
{
    public partial class ApiClient
    {
        public async Task<Profile> GetProfileAsync(
            CancellationToken token = default)
        {
            return await client.GetJsonAsync<Profile>("/api/Global");
        }

        public async Task<bool> ValidateIdentityAsync(
            string username, string password, CancellationToken token = default)
        {
            return (await client.GetStringAsync($"/api/State?usr={username}&pwd={password}")) == "Authenticated";
        }
    }
}
