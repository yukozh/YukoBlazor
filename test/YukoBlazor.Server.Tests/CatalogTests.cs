using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using YukoBlazor.Shared;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class CatalogTests : WebApiTestBase
    {
        [Fact]
        public async Task GetCatalogSummaryTest()
        {
            // Arrange
            await CreateCatalogAsync("catalog-1", "Catalog #1");
            await CreateCatalogAsync("catalog-2", "Catalog #2", 1);
            await CreatePostAsync(
                "test-post",
                "Test Post",
                "### Hello World",
                "",
                "catalog-1");

            // Act
            using (var response = await Client.GetAsync("/api/Catalog"))
            {
                // Assert #1 : The API has no internal errors
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var json = await response.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<IEnumerable<CatalogViewModel>>(json);

                // Assert #2 : The post count of catalog is correct
                Assert.Contains(obj, x => x.Id == "catalog-1" && x.Count == 1);
                Assert.Contains(obj, x => x.Id == "catalog-2" && x.Count == 0);

                // Assert #3 : The catalog returned in order
                Assert.True(obj.First().Id == "catalog-2");
            }
        }

        [Fact]
        public async Task CreateCatalogTest()
        {
            // Arrange
            await CreateCatalogAsync("catalog-1", "Catalog #1");
            await CreateCatalogAsync("catalog-2", "Catalog #2");
            await CreateCatalogAsync("catalog-3", "Catalog #3");

            // Assert
            Assert.Equal(3, await LongCountCatalogsAsync());
        }

        [Fact]
        public async Task CreateCatalogConflictTest()
        {
            // Arrange
            await CreateCatalogAsync("catalog-1", "Catalog #1");
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => CreateCatalogAsync("catalog-1", "Catalog #1"));

            // Assert
            Assert.Equal(1, await LongCountCatalogsAsync());
        }

        [Fact]
        public async Task RemoveExistedCatalogTest()
        {
            // Arrange
            await CreateCatalogAsync("catalog-1", "Catalog #1");
            await CreatePostAsync(
                "test-post", 
                "Test Post", 
                "### Hello World",
                "",
                "catalog-1");

            // Act
            await DeleteCatalogAsync("catalog-1");

            // Assert #1
            Assert.Equal(0, await LongCountCatalogsAsync());

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(
                    "SELECT \"CatalogId\" FROM \"Posts\" WHERE \"Url\" = 'test-post'", 
                    conn))
                {
                    // Assert #2
                    Assert.Equal(DBNull.Value, (DBNull)await cmd.ExecuteScalarAsync());
                }
            }
        }

        [Fact]
        public async Task RemoveNonExistedCatalogTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => DeleteCatalogAsync("catalog-1"));
        }


        [Fact]
        public async Task PatchExistedCatalogTest()
        {
            // Arrange
            await CreateCatalogAsync("catalog-1", "Catalog #1");

            // Act #1 : Modify display text
            await ModifyCatalogAsync("catalog-1", "#1");

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(
                    "SELECT \"Display\" FROM \"Catalogs\" WHERE \"Id\" = 'catalog-1'", 
                    conn))
                {
                    // Assert #1.1 : Validate the display text
                    Assert.Equal("#1", (string)await cmd.ExecuteScalarAsync());
                }

                using (var cmd = new SqliteCommand(
                    "SELECT \"Priority\" FROM \"Catalogs\" WHERE \"Id\" = 'catalog-1'",
                    conn))
                {
                    // Assert #1.2 : Validate the priority is not modified
                    Assert.Equal(0, (long)await cmd.ExecuteScalarAsync());
                }
            }

            // Act  #2 : Modify priority
            await ModifyCatalogAsync("catalog-1", priority: 1);

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(
                    "SELECT \"Priority\" FROM \"Catalogs\" WHERE \"Id\" = 'catalog-1'",
                    conn))
                {
                    // Assert #2 : Validate the priority
                    Assert.Equal(1, (long)await cmd.ExecuteScalarAsync());
                }
            }
        }

        [Fact]
        public async Task PatchNonExistedCatalogTest()
        {
            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => ModifyCatalogAsync("catalog-1", "Catalog #1"));
        }

        private async Task<long> LongCountCatalogsAsync(CancellationToken token = default)
        {
            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync(token);
                using (var cmd = new SqliteCommand(
                    "SELECT COUNT(*) FROM \"Catalogs\"", 
                    conn))
                {
                    return (long)await cmd.ExecuteScalarAsync();
                }
            }
        }
    }
}
