using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using YukoBlazor.Server.Controllers;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace YukoBlazor.Server.Tests
{
    public abstract class WebApiTestBase : IDisposable
    {
        private const string bind = "http://localhost:35543";
        private static readonly Random random = new Random();

        protected readonly IWebHost Host;
        protected readonly static string DbPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"blog.db");
        protected static HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri(bind),
            Timeout = new TimeSpan(0, 0, 3)
        };

        public WebApiTestBase()
        {
            // Ensure there is no db file in test environment
            EnsureDbFileRemoved();

            // Build host
            Host = Program.BuildWebHost(new[] { "-url", bind, "-db", DbPath });

            // Run host
            Host.StartAsync();

            // Wait for API server ready
            WaitHostLaunchAsync(15).Wait();
        }

        public void Dispose()
        {
            Host.Dispose();
            EnsureDbFileRemoved();
        }

        private void EnsureDbFileRemoved()
        {
            if (File.Exists(DbPath))
            {
                File.Delete(DbPath);
            }
        }

        private async Task<bool> WaitHostLaunchAsync(int seconds = -1)
        {
            var timeUsed = 0;
            while (true)
            {
                if (seconds > 0 && timeUsed >= seconds)
                {
                    return false;
                }

                try
                {
                    using (var response = await Client.GetAsync("/api/Hello"))
                    {
                        var text = await response.Content.ReadAsStringAsync();
                        if (text == HomeController.ServiceOkText)
                        {
                            return true;
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("API server is not ready.");
                }

                await Task.Delay(1000);
            }
        }

        #region Manage Catalog
        protected async Task CreateCatalogAsync(
            string url, string text, int priority = 0, CancellationToken token = default)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "display", text },
                { "priority", priority.ToString() }
            });

            using (var response = await Client.PostAsync(
                $"/api/Catalog/{url}?usr=root&pwd=123456", 
                content,
                token))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(error);
                }
            }
        }

        protected async Task ModifyCatalogAsync(
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

            using (var response = await Client.PatchAsync(
                $"/api/Catalog/{url}?usr=root&pwd=123456",
                content,
                token))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(error);
                }
            }
        }

        protected async Task DeleteCatalogAsync(
            string url, CancellationToken token = default)
        {
            using (var response = await Client.DeleteAsync(
                $"/api/Catalog/{url}?usr=root&pwd=123456",
                token))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(error);
                }
            }
        }
        #endregion

        #region Manage Post
        protected async Task<Guid> CreatePostAsync(
            string url, string title, string content,
            string tags, string catalog, bool isPage = false,
            CancellationToken token = default)
        {
            var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "title", title },
                { "content", content },
                { "tags", tags },
                { "catalog", catalog },
                { "isPage", isPage ? "true" : "false" }
            });

            using (var response = await Client.PutAsync(
                $"/api/Post/{url}?usr=root&pwd=123456",
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

        protected async Task ModifyPostAsync(
            string url, string newUrl = null, string title = null, string content = null,
            string catalog = null, string tags = null, bool? isPage = false,
            CancellationToken token = default)
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

            var httpContent = new FormUrlEncodedContent(dictionary);

            using (var response = await Client.PatchAsync(
                $"/api/Post/{url}?usr=root&pwd=123456",
                httpContent,
                token))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(error);
                }
            }
        }

        protected async Task DeletePostAsync(
            string url, CancellationToken token = default)
        {
            using (var response = await Client.DeleteAsync(
                $"/api/Post/{url}?usr=root&pwd=123456",
                token))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(error);
                }
            }
        }
        #endregion
    }
}
