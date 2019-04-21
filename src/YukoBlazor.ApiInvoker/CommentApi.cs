using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using YukoBlazor.Shared;

namespace YukoBlazor.ApiInvoker
{
    public partial class ApiClient
    {
        public async Task<IEnumerable<CommentViewModel>> GetCommentsAsync(
            Guid id, CancellationToken token = default)
        {
            return await client.GetJsonAsync<IEnumerable<CommentViewModel>>("/api/Comment/" + id);
        }

        public async Task<Guid> PutCommentAsync(
            Guid id, string content, string name = null, string email = null,
            bool isRootComment = true, CancellationToken token = default)
        {
            if (!container.IsAuthenticated)
            {
                if (name == null)
                {
                    throw new MissingFieldException(nameof(name));
                }
                else if (email == null)
                {
                    throw new MissingFieldException(nameof(email));
                }
            }

            using (var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "content", content },
                { "isRootComment", isRootComment ? "true" : "false"},
                { "name", name },
                { "email", email }
            }))
            using (var response = await client.PostAsync(
                $"/api/Comment/{id}",
                httpContent,
                token))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(error);
                }
                else
                {
                    return JsonConvert.DeserializeObject<Guid>(
                        await response.Content.ReadAsStringAsync());
                }
            }
        }

        protected async Task DeleteCommentAsync(
            Guid id, CancellationToken token = default)
        {
            using (var response = await client.DeleteAsync(
                $"/api/Comment/{id}",
                token))
            {
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidCredentialException(error);
                }
                else if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(error);
                }
            }
        }
    }
}
