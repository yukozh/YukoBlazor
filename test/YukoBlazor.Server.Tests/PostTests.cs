using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Linq;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using YukoBlazor.Shared;
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
        public async Task PatchPartialFieldsWithExistedPostTest()
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
        public async Task PatchAllFieldsWithExistedPostTest()
        {
            // Arrange
            const string postUrl = "test-post";
            var guid = await CreatePostAsync(
                postUrl,
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);
            await CreateCatalogAsync("catalog-1", "Catalog #1");

            // Act
            await ModifyPostAsync(
                postUrl, "new-url", "New Title", 
                "New Content", "catalog-1", "", false);

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(
                    $"SELECT \"CatalogId\" FROM \"Posts\" WHERE \"Id\" = @p1",
                    conn))
                {
                    cmd.Parameters.Add(new SqliteParameter("p1", guid));

                    // Assert #1
                    Assert.Equal("catalog-1", (string)await cmd.ExecuteScalarAsync());
                }
            }
        }

        [Fact]
        public async Task PatchPostUrlConflictTest()
        {
            // Arrange
            var guid = await CreatePostAsync(
                "test-post-1",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            var guid2 = await CreatePostAsync(
                "test-post-2",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => ModifyPostAsync("test-post-1", "test-post-2"));
        }

        [Fact]
        public async Task PatchPostWrongCatalogConflictTest()
        {
            // Arrange
            var guid = await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => ModifyPostAsync("test-post", catalog: "some-catalog"));
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

        [Fact]
        public async Task GuestRemovePostTest()
        {
            // Arrange
            await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Act
            using (var response = await Client.DeleteAsync(
                $"/api/Post/test-post"))
            {
                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        [Fact]
        public async Task GuestPatchPostTest()
        {
            // Arrange
            await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);
            var content = new FormUrlEncodedContent(new Dictionary<string, string>());

            // Act
            using (var response = await Client.PatchAsync(
                $"/api/Post/test-post", content))
            {
                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        [Fact]
        public async Task GuestPutPostTest()
        {
            // Arrange
            var content = new FormUrlEncodedContent(new Dictionary<string, string>());

            // Act
            using (var response = await Client.PutAsync(
                $"/api/Post/test-post", content))
            {
                // Assert
                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("title=Test", true)]
        [InlineData("title=Some", false)]
        [InlineData("tag=A", true)]
        [InlineData("tag=D", false)]
        [InlineData("catalog=catalog-1", true)]
        [InlineData("catalog=catalog-2", false)]
        [InlineData("isPage=true", false)]
        [InlineData("from=1970-01-01", true)]
        [InlineData("to=1970-01-01", false)]
        public async Task GetPostListTest(string condition, bool containsResult)
        {
            // Arrange
            await CreateCatalogAsync("catalog-1", "Catalog #1");
            await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                "catalog-1");

            // Act
            using (var response = await Client.GetAsync(
                $"/api/Post?" + condition))
            {
                // Assert #1
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var result = JsonConvert.DeserializeObject<PagedViewModel<PostViewModel>>(
                    await response.Content.ReadAsStringAsync());

                // Assert #2
                if (containsResult)
                {
                    Assert.Single(result.Data);
                }
                else
                {
                    Assert.Empty(result.Data);
                }
            }
        }

        [Fact]
        public async Task GetOneExistedPostTest()
        {
            // Arrange
            await CreateCatalogAsync("catalog-1", "Catalog #1");
            await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                "catalog-1");

            // Act
            using (var response = await Client.GetAsync(
                $"/api/Post/test-post"))
            {
                // Assert #1
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var result = JsonConvert.DeserializeObject<PostViewModel>(
                    await response.Content.ReadAsStringAsync());

                Assert.NotNull(result);
                Assert.Equal("Test Post", result.Title);
                Assert.Equal(3, result.Tags.Count());
            }
        }

        [Fact]
        public async Task GetOneNonExistedPostTest()
        {
            // Act
            using (var response = await Client.GetAsync(
                $"/api/Post/test-post"))
            {
                // Assert #1
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task GetCalendarTests()
        {
            // Arrange
            await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                null,
                null);
            await CreatePostAsync(
                "test-post-2",
                "Test Post 2",
                "### Hello World",
                null,
                null);

            // Act
            using (var response = await Client.GetAsync("/api/Calendar"))
            {
                var obj = JsonConvert.DeserializeObject<IEnumerable<CalendarViewModel>>(
                    await response.Content.ReadAsStringAsync());

                // Assert
                Assert.Single(obj);

                var item = obj.First();
                Assert.Equal(DateTime.UtcNow.Year, item.Year);
                Assert.Equal(DateTime.UtcNow.Month, item.Month);
                Assert.Equal(2, item.Count);
            }
        }

        [Fact]
        public async Task GetTagTests()
        {
            // Arrange
            await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "A, B, C",
                null);

            // Act
            using (var response = await Client.GetAsync("/api/Tag"))
            {
                var obj = JsonConvert.DeserializeObject<IEnumerable<TagViewModel>>(
                    await response.Content.ReadAsStringAsync());

                // Assert
                Assert.Equal(3, obj.Count());
                Assert.Single(obj, x => x.Title == "A" && x.Count == 1);
                Assert.Single(obj, x => x.Title == "B" && x.Count == 1);
                Assert.Single(obj, x => x.Title == "C" && x.Count == 1);
            }
        }
    }
}
