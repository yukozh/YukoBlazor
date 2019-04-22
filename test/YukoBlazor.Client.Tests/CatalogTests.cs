using System.Threading.Tasks;
using OpenQA.Selenium;
using Xunit;

namespace YukoBlazor.Client.Tests
{
    public class CatalogTests : FrontendTestBase
    {
        [Fact]
        public async Task ManageCatalogTest()
        {
            // Arrange
            await LoginAsync("root", "123456");
            var catalogManageLink = await WaitForElementAsync(By.Id("link-manage-catalog"));
            catalogManageLink.Click();
            var txtUrl = await WaitForElementAsync(By.Id("textbox-add-url"));
            txtUrl.SendKeys("test-catalog-1");
            var txtDisplay = await WaitForElementAsync(By.Id("textbox-add-display"));
            txtDisplay.SendKeys("Test #1");
            var txtPriority = await WaitForElementAsync(By.Id("textbox-add-priority"));
            txtPriority.SendKeys("10");

            // Act
            var btnAddCatalog = await WaitForElementAsync(By.Id("button-add-catalog"));
            btnAddCatalog.Click();

            // Assert
            Assert.NotNull(await WaitForElementAsync(By.LinkText("Test #1")));

            // Arrange
            var txtBox = await WaitForElementAsync(By.ClassName("textbox"));
            txtBox.Clear();
            txtBox.SendKeys("Test Catalog");

            // Act
            var btnSave = await WaitForElementAsync(By.ClassName("button-save-catalog"));
            btnSave.Click();

            // Assert
            var catalogLink = await WaitForElementAsync(By.LinkText("Test Catalog"));
            Assert.NotNull(catalogLink);

            // Act
            var btnDelete = await WaitForElementAsync(By.ClassName("button-delete-catalog"));
            btnDelete.Click();

            // Assert
            Assert.True(await WaitForElementDisappearAsync(catalogLink, 1000));
        }
    }
}
