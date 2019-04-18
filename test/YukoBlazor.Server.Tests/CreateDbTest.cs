using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using YukoBlazor.Server.Controllers;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class CreateDbTest : IDisposable
    {
        private IWebHost host;
        private const string bind = "http://localhost:35543";
        private static readonly string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "blog.db");
        private static HttpClient client = new HttpClient
        {
            BaseAddress = new Uri(bind),
            Timeout = new TimeSpan(0, 0, 3)
        };

        public CreateDbTest()
        {
            // Ensure there is no db file in test environment
            EnsureDbFileRemoved();

            // Build host
            host = Program.BuildWebHost(new[] { "-url", bind, "-db", dbPath });

            // Run host
            host.StartAsync();

            // Wait for API server ready
            WaitHostLaunchAsync(15).Wait();
        }

        [Fact]
        public void DbFileCreateTests()
        {
            Assert.True(File.Exists(dbPath));
        }

        public void Dispose()
        {
            EnsureDbFileRemoved();
        }

        private void EnsureDbFileRemoved()
        {
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
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
                    using (var response = await client.GetAsync("/"))
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

    }
}
