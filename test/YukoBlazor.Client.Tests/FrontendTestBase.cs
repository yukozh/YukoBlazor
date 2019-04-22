using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Authentication;
using Microsoft.AspNetCore.Hosting;
using YukoBlazor.Server.Controllers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace YukoBlazor.Client.Tests
{
    public class FrontendTestBase : IDisposable
    {
        private const string bind = "http://localhost:35544";

        protected readonly IWebHost Host;
        protected readonly IWebDriver WebDriver;
        protected static HttpClient Client = new HttpClient
        {
            BaseAddress = new Uri(bind),
            Timeout = new TimeSpan(0, 0, 3)
        };
        protected readonly static string DbPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"blog.db");

        protected string Bind => bind;

        public FrontendTestBase()
        {
            // Ensure there is no db file in test environment
            EnsureDbFileRemoved();

            // Build host
            Host = Server.Program.BuildWebHost(new[] { "-url", bind, "-db", DbPath });

            // Run host
            Host.StartAsync();

            // Wait for API server ready
            WaitHostLaunchAsync(15).Wait();

            WebDriver = new ChromeDriver();
            WebDriver.Url = bind;
        }

        public async Task<IWebElement> WaitForElementAsync(By by, int timeout = 30000)
        {
            var timeused = 0;
            while (true)
            {
                if (timeused > timeout)
                {
                    throw new TimeoutException();
                }

                try
                {
                    var element = WebDriver.FindElement(by);
                    if (element != null)
                    {
                        return element;
                    }
                }
                catch (NoSuchElementException)
                {
                    await Task.Delay(200);
                    timeused += 200;
                    continue;
                }
            }
        }

        public async Task LoginAsync(string username, string password)
        {
            WebDriver.Url = Bind + "/manage/login";
            var txtUsername = await WaitForElementAsync(By.Id("textbox-username"));
            txtUsername.SendKeys(username);
            var txtPassword = await WaitForElementAsync(By.Id("textbox-password"));
            txtPassword.SendKeys(password);

            var btnLogin = await WaitForElementAsync(By.Id("button-login"));
            btnLogin.Click();

            try
            {
                await WaitForElementAsync(By.Id("sidebar-manage"), 1000);
            }
            catch(TimeoutException)
            {
                throw new InvalidCredentialException();
            }

            return;
        }

        public void Dispose()
        {
            WebDriver?.Dispose();
            Host?.Dispose();
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
    }
}
