using System.Threading.Tasks;
using OpenQA.Selenium;
using Xunit;

namespace YukoBlazor.Client.Tests
{
    public class PostTests : FrontendTestBase
    {
        [Fact]
        public async Task PostTest()
        {
            // Arrange
            await LoginAsync("root", "123456");
            var btnCreatePost = await WaitForElementAsync(By.Id("link-manage-post"));
            btnCreatePost.Click();
            var txtUrl = await WaitForElementAsync(By.Id("textbox-post-url"));
            txtUrl.SendKeys("test-post-1");
            var txtTitle = await WaitForElementAsync(By.Id("textbox-post-title"));
            txtTitle.SendKeys("This is a test post #1");
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            js.ExecuteScript("$('#textarea-txtPost')[0].smde.value('Test Content')");

            // Act
            var btnSubmit = await WaitForElementAsync(By.Id("button-post-submit"));
            btnSubmit.Click();

            // Assert
            Assert.NotNull(await WaitForElementAsync(By.LinkText("This is a test post #1"), 400));
        }
    }
}
