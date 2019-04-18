using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Data.Sqlite;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class PostTests : WebApiTestBase
    {
        [Fact]
        public async Task CreatePostWithTagsTest()
        {
            // Arrange
            const string postUrl = "test-post";
            var guid = await CreatePostAsync(
                postUrl,
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(
                    $"SELECT \"Url\" FROM \"Posts\" WHERE \"Id\" = @p1",
                    conn))
                {
                    cmd.Parameters.Add(new SqliteParameter("p1", guid));

                    // Assert #1 : Validate the post is inserted into database
                    Assert.Equal(postUrl, (string)await cmd.ExecuteScalarAsync());
                }

                using (var cmd = new SqliteCommand(
                    $"SELECT COUNT(*) FROM \"PostTags\" WHERE \"PostId\" = @p1",
                    conn))
                {
                    cmd.Parameters.Add(new SqliteParameter("p1", guid));

                    // Assert #2 : Ensure the tags have been added
                    Assert.Equal(3, (long)await cmd.ExecuteScalarAsync());
                }
            }
        }

        [Fact]
        public async Task CreatePostConflictTest()
        {
            // Arrange
            const string postUrl = "test-post";
            var guid = await CreatePostAsync(
                postUrl,
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreatePostAsync(
                    postUrl,
                    "Test Post",
                    "### Hello World",
                    "A, B, C",
                    null));

        }

        [Fact]
        public async Task CreatePostWithNonExistedCatalogTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreatePostAsync(
                    "test-post",
                    "Test Post",
                    "### Hello World",
                    "A, B, C",
                    "non-existed-catalog"));
        }

        [Fact]
        public async Task PatchExistedPostTest()
        {
            // Arrange
            const string postUrl = "test-post";
            var guid = await CreatePostAsync(
                postUrl,
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Act
            const string newTitle = "New Title";
            const string newContent = "### New content";
            await ModifyPostAsync(postUrl, title: newTitle, content: newContent, tags: "D");

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(
                    $"SELECT \"Title\" FROM \"Posts\" WHERE \"Id\" = @p1",
                    conn))
                {
                    cmd.Parameters.Add(new SqliteParameter("p1", guid));

                    // Assert #1
                    Assert.Equal(newTitle, (string)await cmd.ExecuteScalarAsync());
                }

                using (var cmd = new SqliteCommand(
                    $"SELECT \"Content\" FROM \"Posts\" WHERE \"Id\" = @p1",
                    conn))
                {
                    cmd.Parameters.Add(new SqliteParameter("p1", guid));

                    // Assert #2
                    Assert.Equal(newContent, (string)await cmd.ExecuteScalarAsync());
                }

                using (var cmd = new SqliteCommand(
                    $"SELECT COUNT(*) FROM \"PostTags\" WHERE \"PostId\" = @p1",
                    conn))
                {
                    cmd.Parameters.Add(new SqliteParameter("p1", guid));

                    // Assert #3
                    Assert.Equal(1, (long)await cmd.ExecuteScalarAsync());
                }
            }
        }

        [Fact]
        public async Task PatchNonExistedPostTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => ModifyPostAsync("some-post", "some-post-2"));
        }

        [Fact]
        public async Task DeleteExistedPostTest()
        {
            // Arrange
            const string postUrl = "test-post";
            var guid = await CreatePostAsync(
                postUrl,
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Act
            await DeletePostAsync(postUrl);

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(
                    $"SELECT COUNT(*) FROM \"Posts\" WHERE \"Id\" = @p1",
                    conn))
                {
                    cmd.Parameters.Add(new SqliteParameter("p1", guid));

                    // Assert #1 : Validate the post is inserted into database
                    Assert.Equal(0, (long)await cmd.ExecuteScalarAsync());
                }
            }
        }

        [Fact]
        public async Task DeleteNonExistedPostTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => DeletePostAsync("some-post"));
        }
    }
}
