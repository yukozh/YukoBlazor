using System.Threading.Tasks;
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
            WebDriver.Url = Bind + "/manage/login";
            var txtUsername = await WaitForElementAsync(By.Id("textbox-username"));
            txtUsername.SendKeys("root");
            var txtPassword = await WaitForElementAsync(By.Id("textbox-password"));
            txtPassword.SendKeys("123456");

            // Act
            var btnLogin = await WaitForElementAsync(By.Id("button-login"));
            btnLogin.Click();

            // Assert
            Assert.NotNull(await WaitForElementAsync(By.Id("sidebar-manage")));
        }
    }
}
