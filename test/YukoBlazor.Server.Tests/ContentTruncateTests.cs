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
            var content = @"### Hello world";
            var truncated = PostController.TruncateContent(content);
            Assert.Equal(content, truncated);
        }

        [Fact]
        public void ContentLinesEqualToLimitation()
        {
            var input = new StringBuilder();
            for (var i = 0; i < 10; ++i)
            {
                input.AppendLine("test");
            }
            var expected = input.ToString();
            var truncated = PostController.TruncateContent(input.ToString());
            Assert.Equal(expected, truncated);
        }

        [Fact]
        public void ContentLinesGreaterThanLimitation()
        {
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
            var truncated = PostController.TruncateContent(input.ToString());
            Assert.Equal(expected, truncated);
        }
    }
}
