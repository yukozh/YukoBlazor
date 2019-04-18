using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class CatalogTests : WebApiTestBase
    {
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
            await DeleteCatalogAsync("catalog-1");

            // Assert
            Assert.Equal(0, await LongCountCatalogsAsync());
        }


        [Fact]
        public async Task PatchExistedCatalogTest()
        {
            // Arrange
            await CreateCatalogAsync("catalog-1", "Catalog #1");

            // Act
            await ModifyCatalogAsync("catalog-1", "#1");

            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand("SELECT \"Display\" FROM \"Catalogs\" WHERE \"Id\" = 'catalog-1'", conn))
                {
                    // Assert
                    Assert.Equal("#1", (string)await cmd.ExecuteScalarAsync());
                }
            }
        }

        private async Task<long> LongCountCatalogsAsync(CancellationToken token = default)
        {
            using (var conn = new SqliteConnection($"Data source={DbPath}"))
            {
                await conn.OpenAsync(token);
                using (var cmd = new SqliteCommand("SELECT COUNT(*) FROM \"Catalogs\"", conn))
                {
                    return (long)await cmd.ExecuteScalarAsync();
                }
            }
        }
    }
}
