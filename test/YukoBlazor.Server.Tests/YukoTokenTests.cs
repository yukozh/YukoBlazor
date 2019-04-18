using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using YukoBlazor.Server.Controllers;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class YukoTokenTests : WebApiTestBase
    {
        [Fact]
        public async Task NotAuthenticatedTest()
        {
            using (var resposne = await client.GetAsync("/api/State"))
            {
                var text = await resposne.Content.ReadAsStringAsync();
                Assert.Equal(HomeController.NotAuthenticated, text);
            }
        }

        [Theory]
        [InlineData("root", "654321")]
        [InlineData("wrong", "123456")]
        [InlineData("wrong", "654321")]
        public async Task AuthenticateFailedWithQueryStringTest(string usr, string pwd)
        {
            using (var resposne = await client.GetAsync($"/api/State?usr={usr}&pwd={pwd}"))
            {
                var text = await resposne.Content.ReadAsStringAsync();
                Assert.Equal(HomeController.NotAuthenticated, text);
            }
        }

        [Fact]
        public async Task AuthenticateSuccessWithQueryStringTest()
        {
            using (var resposne = await client.GetAsync("/api/State?usr=root&pwd=123456"))
            {
                var text = await resposne.Content.ReadAsStringAsync();
                Assert.Equal(HomeController.Authenticated, text);
            }
        }
    }
}
