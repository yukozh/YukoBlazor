using System.Threading.Tasks;
using System.Security.Authentication;
using OpenQA.Selenium;
using Xunit;

namespace YukoBlazor.Client.Tests
{
    public class LoginTests : FrontendTestBase
    {
        [Fact]
        public async Task LoginSuccessTest()
        {
            // Arrange
            await LoginAsync("root", "123456");

            // Assert
            Assert.NotNull(await WaitForElementAsync(By.Id("sidebar-manage")));
        }

        [Fact]
        public async Task LoginFailedTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidCredentialException>(
                () => LoginAsync("wrong", "654321"));

            // TODO: Enhance login failed UX, assert tip
        }
    }
}
