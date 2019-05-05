using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using YukoBlazor.Shared;

namespace YukoBlazor.ApiInvoker
{
    public partial class ApiClient
    {
        public async Task<IEnumerable<TagViewModel>> GetTagsAsync(
            CancellationToken token = default)
        {
            return await client.GetJsonAsync<IEnumerable<TagViewModel>>("api/Tag");
        }
    }
}
