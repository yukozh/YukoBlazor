using System.Threading.Tasks;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class ProfileTests : WebApiTestBase
    {
        [Fact]
        public async Task GetProfileTest()
        {
            // Arrange
            var profile = await API.GetProfileAsync();

            // Assert
            Assert.Equal("Yuko", profile.BlogTitle);
            Assert.Equal("Based on Blazor", profile.Subtitle);
            Assert.Equal("Yuko", profile.Nickname);
            Assert.Equal("hi@yuko.me", profile.Email);
            Assert.Equal("yukozh", profile.GitHub);
        }
    }
}
