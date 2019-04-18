using System;
using System.Text;
using YukoBlazor.Server.Controllers;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class ContentTruncateTests
    {
        [Fact]
        public void ContentLinesLowerThanLimitation()
        {
            // Arrange
            var content = @"### Hello world";

            // Act
            var truncated = PostController.TruncateContent(content);

            // Assert
            Assert.Equal(content, truncated);
        }

        [Fact]
        public void ContentLinesEqualToLimitation()
        {
            // Arrange
            var input = new StringBuilder();
            for (var i = 0; i < 10; ++i)
            {
                input.AppendLine("test");
            }
            var expected = input.ToString();

            // Act
            var truncated = PostController.TruncateContent(input.ToString());

            // Assert
            Assert.Equal(expected, truncated);
        }

        [Fact]
        public void ContentLinesGreaterThanLimitation()
        {
            // Arrange
            var input = new StringBuilder();
            for (var i = 0; i < 10; ++i)
            {
                input.AppendLine("test");
            }
            var expected = input.ToString();
            for (var i = 0; i < 5; ++i)
            {
                input.AppendLine("exceed");
            }

            // Act
            var truncated = PostController.TruncateContent(input.ToString());

            // Assert
            Assert.Equal(expected, truncated);
        }
    }
}
