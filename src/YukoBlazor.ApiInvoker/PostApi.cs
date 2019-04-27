using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Security.Authentication;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using YukoBlazor.Shared;

namespace YukoBlazor.ApiInvoker
{
    public partial class ApiClient
    {
        public async Task<PagedViewModel<PostViewModel>> GetPostListAsync(
            string catalog = null, string tag = null, int? year = null,
            int? month = null, string search = null, bool isPage = false, int page = 1,
            CancellationToken token = default)
        {
            var conditionBuilder = new StringBuilder($"?isPage={isPage}&page={page}");

            if (!string.IsNullOrEmpty(catalog))
            {
                conditionBuilder.Append($"&catalog={catalog}");
            }

            if (!string.IsNullOrEmpty(tag))
            {
                conditionBuilder.Append($"&tag={tag}");
            }

            if (year.HasValue && month.HasValue)
            {
                conditionBuilder.Append($"&from={new DateTime(year.Value, month.Value, 1).ToString("yyyy-MM-dd")}");
                conditionBuilder.Append($"&to={new DateTime(year.Value, month.Value, 1).AddMonths(1).ToString("yyyy-MM-dd")}");
            }

            if (!string.IsNullOrEmpty(search))
            {
                conditionBuilder.Append($"&title={HttpUtility.UrlEncode(search)}");
            }

            return await client.GetJsonAsync<PagedViewModel<PostViewModel>>("/api/Post/" + conditionBuilder.ToString());
        }

        public async Task<PostViewModel> GetPostAsync(string url, CancellationToken token = default)
        {
            return await client.GetJsonAsync<PostViewModel>($"/api/Post/{url}");
        }

        public async Task<Guid> PutPostAsync(
            string url, string title, string content, string catalog = null, 
            string tags = null, bool isPage = false, bool isFeatured = false,
            CancellationToken token = default)
        {
            using (var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "title", title },
                { "content", content },
                { "tags", tags },
                { "catalog", catalog },
                { "isPage", isPage ? "true" : "false" },
                { "isFeatured", isFeatured ? "true" : "false" }
            }))
            using (var response = await client.PutAsync(
                $"/api/Post/{url}",
                httpContent,
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
                else
                {
                    return JsonConvert.DeserializeObject<Guid>(
                        await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task PatchPostAsync(
            string url, string newUrl = null, string title = null, string content = null,
            string catalog = null, string tags = null, bool? isPage = null,
            bool? isFeatured = null, CancellationToken token = default)
        {
            var dictionary = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(newUrl))
            {
                dictionary.Add("newUrl", newUrl);
            }

            if (!string.IsNullOrEmpty(title))
            {
                dictionary.Add("title", title);
            }

            if (!string.IsNullOrEmpty(content))
            {
                dictionary.Add("content", content);
            }

            if (!string.IsNullOrEmpty(catalog))
            {
                dictionary.Add("catalog", catalog);
            }

            if (!string.IsNullOrEmpty(tags))
            {
                dictionary.Add("tags", tags);
            }

            if (isPage.HasValue)
            {
                dictionary.Add("isPage", isPage.ToString());
            }

            if (isFeatured.HasValue)
            {
                dictionary.Add("isFeatured", isFeatured.ToString());
            }

            using (var httpContent = new FormUrlEncodedContent(dictionary))
            using (var response = await client.PatchAsync(
                $"/api/Post/{url}",
                httpContent,
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

        public async Task DeletePostAsync(
            string url, CancellationToken token = default)
        {
            using (var response = await client.DeleteAsync(
                $"/api/Post/{url}",
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
