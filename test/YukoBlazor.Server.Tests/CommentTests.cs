using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using YukoBlazor.Shared;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class CommentTests : WebApiTestBase
    {
        [Fact]
        public async Task PutCommentTest()
        {
            // Arrange
            var postId = await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Act
            var commentId = await CreateCommentAsync(postId, "Test #1", "Cat", "cat@yuko.me");
            var subCommentId = await CreateCommentAsync(commentId, "Test #2", "Dog", "dog@yuko.me", false);

            using (var response = await Client.GetAsync("/api/Comment/" + postId))
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await response.Content.ReadAsStringAsync();
                // Assert
                var result = JsonConvert.DeserializeObject<IEnumerable<CommentViewModel>>(text);
                Assert.Single(result, x => x.Id == commentId);
                Assert.Single(result.Single().InnerComments, x => x.Id == subCommentId);
            }
        }
    }
}
