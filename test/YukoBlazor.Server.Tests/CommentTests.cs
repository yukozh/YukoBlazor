using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using YukoBlazor.Shared;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class CommentTests : WebApiTestBase
    {
        [Fact]
        public async Task PutCommentToExistedPostTest()
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
                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<CommentViewModel>>(text);
                Assert.Single(result, x => x.Id == commentId);
                Assert.Single(result.Single().InnerComments, x => x.Id == subCommentId);
            }
        }

        [Fact]
        public async Task PutCommentToNonExistedPostTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreateCommentAsync(default, "Test #1", "Cat", "cat@yuko.me"));
        }

        [Fact]
        public async Task PutCommentToNonExistedParentTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreateCommentAsync(default, "Test #1", "Cat", "cat@yuko.me", false));
        }

        [Fact]
        public async Task PutCommentAsOwnerTest()
        {
            // Arrange
            var postId = await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Act
            var commentId = await CreateCommentAsync(postId, "Test #1", "Cat", "cat@yuko.me", true, true);

            using (var response = await Client.GetAsync("/api/Comment/" + postId))
            {
                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<CommentViewModel>>(text);
                Assert.Single(result, x => x.Id == commentId);
                Assert.Equal("Yuko", result.Single().Name);
            }
        }

        [Fact]
        public async Task DeleteExistedCommentTest()
        {
            // Arrange
            var postId = await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);
            var commentId = await CreateCommentAsync(postId, "Test #1", "Cat", "cat@yuko.me", true, true);

            // Act
            await DeleteCommentAsync(commentId);

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(
                    $"SELECT COUNT(*) FROM \"Comments\" WHERE \"Id\" = @p1",
                    conn))
                {
                    cmd.Parameters.Add(new SqliteParameter("p1", commentId));

                    // Assert
                    Assert.Equal(0, (long)await cmd.ExecuteScalarAsync());
                }
            }
        }

        [Fact]
        public async Task DeleteNonExistedCommentTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => DeleteCommentAsync(default));
        }

        [Fact]
        public async Task GuestDeleteCommentTest()
        {
            using (var response = await Client.DeleteAsync(
                $"/api/Comment/{default(Guid)}"))
            {
                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }
    }
}
