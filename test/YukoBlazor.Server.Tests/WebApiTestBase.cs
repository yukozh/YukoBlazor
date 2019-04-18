using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
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

        protected void CreateCatalog(string url, string text)
        {

        }
    }
}
