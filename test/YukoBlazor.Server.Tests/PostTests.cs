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
        public async Task CreatePostWithNonExistedCatalogTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await CreatePostAsync(
                    "test-post",
                    "Test Post",
                    "### Hello World",
                    "A, B, C",
                    "non-existed-catalog"));
        }
    }
}
