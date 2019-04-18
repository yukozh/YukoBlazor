using System.Threading.Tasks;
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

        [Theory]
        [InlineData("root", "654321")]
        [InlineData("wrong", "123456")]
        [InlineData("wrong", "654321")]
        public async Task AuthenticateFailedWithHeaderTest(string usr, string pwd)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Yuko {usr} {pwd}");
            try
            {
                using (var resposne = await client.GetAsync("/api/State"))
                {
                    var text = await resposne.Content.ReadAsStringAsync();
                    Assert.Equal(HomeController.NotAuthenticated, text);
                }
            }
            finally
            {
                client.DefaultRequestHeaders.Remove("Authorization");
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

        [Fact]
        public async Task AuthenticateSuccessWithHeaderTest()
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Yuko root 123456");
            try
            {
                using (var resposne = await client.GetAsync("/api/State"))
                {
                    var text = await resposne.Content.ReadAsStringAsync();
                    Assert.Equal(HomeController.Authenticated, text);
                }
            }
            finally
            {
                client.DefaultRequestHeaders.Remove("Authorization");
            }
        }
    }
}
