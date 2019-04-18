using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class CreateDbTests : WebApiTestBase
    {
        [Fact]
        public void DbFileCreateTests()
        {
            Assert.True(File.Exists(dbPath));

            Exception exception = null;
            try
            {
                using (var conn = new SqliteConnection("Data source=" + dbPath))
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.Null(exception);
        }
    }
}
