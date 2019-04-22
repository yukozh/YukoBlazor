using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using Microsoft.AspNetCore.Components;
using YukoBlazor.Shared;

namespace YukoBlazor.ApiInvoker
{
    public partial class ApiClient
    {
        public async Task<IEnumerable<CatalogViewModel>> GetCatalogsAsync(
            CancellationToken token = default)
        {
            return await client.GetJsonAsync<IEnumerable<CatalogViewModel>>("/api/Catalog");
        }

        public async Task PutCatalogAsync(
            string url, string text, int priority = 0, CancellationToken token = default)
        {
            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "display", text },
                { "priority", priority.ToString() }
            }))
            using (var response = await client.PostAsync(
                $"/api/Catalog/{url}",
                content,
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

        public async Task PatchCatalogAsync(
            string url, string text = null, int? priority = null,
            CancellationToken token = default)
        {
            var dictionary = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(text))
            {
                dictionary.Add("display", text);
            }

            if (priority.HasValue)
            {
                dictionary.Add("priority", priority.ToString());
            }

            var content = new FormUrlEncodedContent(dictionary);

            using (var response = await client.PatchAsync(
                $"/api/Catalog/{url}",
                content,
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

        public async Task DeleteCatalogAsync(
            string url, CancellationToken token = default)
        {
            using (var response = await client.DeleteAsync(
                $"/api/Catalog/{url}",
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
