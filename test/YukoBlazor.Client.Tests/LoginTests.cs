using System.Threading.Tasks;
using System.Security.Authentication;
using OpenQA.Selenium;
using Xunit;

namespace YukoBlazor.Client.Tests
{
    public class LoginTests : FrontendTestBase
    {
        [Fact]
        public async Task LoginAndLogoutTest()
        {
            // Arrange
            await LoginAsync("root", "123456");

            // Assert
            var manageLabel = await WaitForElementAsync(By.Id("sidebar-manage"));
            Assert.NotNull(manageLabel);

            // Arrange
            var btnLogout = await WaitForElementAsync(By.Id("link-manage-logout"));

            // Act
            btnLogout.Click();

            // Assert
            Assert.True(await WaitForElementDisappearAsync(manageLabel));
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
