using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using YukoBlazor.Server.Controllers;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class FileUploadTests : WebApiTestBase
    {
        [Fact]
        public async Task InvalidFormatTest()
        {
            // Arrange
            using (var response = await Client.PostAsync("/api/File", new StringContent("")))
            {
                // Assert
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task GuestUploadSuccessViaBase64StringTest()
        {
            // Arrange
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "base64", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAACXBIWXMAAAsTAAALEwEAmpwYAAAE82lUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS42LWMxNDIgNzkuMTYwOTI0LCAyMDE3LzA3LzEzLTAxOjA2OjM5ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0RXZ0PSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VFdmVudCMiIHhtbG5zOnBob3Rvc2hvcD0iaHR0cDovL25zLmFkb2JlLmNvbS9waG90b3Nob3AvMS4wLyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ0MgMjAxOCAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDE5LTA0LTE4VDE5OjU0OjAzKzA4OjAwIiB4bXA6TWV0YWRhdGFEYXRlPSIyMDE5LTA0LTE4VDE5OjU0OjAzKzA4OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAxOS0wNC0xOFQxOTo1NDowMyswODowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6MWY1ZDc0M2QtMGE1Ny02ODRkLWIxZmEtY2VjYWYxMmJlYWVkIiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjFmNWQ3NDNkLTBhNTctNjg0ZC1iMWZhLWNlY2FmMTJiZWFlZCIgeG1wTU06T3JpZ2luYWxEb2N1bWVudElEPSJ4bXAuZGlkOjFmNWQ3NDNkLTBhNTctNjg0ZC1iMWZhLWNlY2FmMTJiZWFlZCIgcGhvdG9zaG9wOkNvbG9yTW9kZT0iMyI+IDx4bXBNTTpIaXN0b3J5PiA8cmRmOlNlcT4gPHJkZjpsaSBzdEV2dDphY3Rpb249ImNyZWF0ZWQiIHN0RXZ0Omluc3RhbmNlSUQ9InhtcC5paWQ6MWY1ZDc0M2QtMGE1Ny02ODRkLWIxZmEtY2VjYWYxMmJlYWVkIiBzdEV2dDp3aGVuPSIyMDE5LTA0LTE4VDE5OjU0OjAzKzA4OjAwIiBzdEV2dDpzb2Z0d2FyZUFnZW50PSJBZG9iZSBQaG90b3Nob3AgQ0MgMjAxOCAoV2luZG93cykiLz4gPC9yZGY6U2VxPiA8L3htcE1NOkhpc3Rvcnk+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+Md1XMwAAAAxJREFUCJlj+M/AAAADAQEAnOO/WQAAAABJRU5ErkJggg==" } 
            });

            // Act
            using (var response = await Client.PostAsync("/api/File", content))
            {
                //Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task GuestUploadSuccessWithFormFileTest()
        {
            // Arrange
            var blob = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new ByteArrayContent(blob), "file", "file.bin");

                // Act
                using (var response = await Client.PostAsync("/api/File", content))
                {
                    var text = await response.Content.ReadAsStringAsync();

                    Exception exception = null;
                    try
                    { 
                        var guid = JsonConvert.DeserializeObject<Guid>(text);
                    }
                    catch(Exception ex)
                    {
                        exception = ex;
                    }

                    // Assert
                    Assert.Null(exception);
                }
            }
        }

        [Fact]
        public async Task GuestUploadFileExceedsLimitationTest()
        {
            // Arrange
            var blob = new byte[1024 * 512 + 1];
            for (var i = 0; i < blob.Length; ++i)
            {
                blob[i] = 0x01;
            }

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new ByteArrayContent(blob), "file", "file.bin");

                // Act
                using (var response = await Client.PostAsync("/api/File", content))
                {
                    // Assert
                    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
                }
            }
        }

        [Fact]
        public async Task OwnerUploadFileExceedsLimitationTest()
        {
            // Arrange
            var blob = new byte[FileController.GuestLimit + 1];
            for (var i = 0; i < blob.Length; ++i)
            {
                blob[i] = 0x01;
            }

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new ByteArrayContent(blob), "file", "file.bin");
                Client.DefaultRequestHeaders.Add("Authorization", $"Yuko root 123456");
                try
                { 
                    // Act
                    using (var response = await Client.PostAsync("/api/File", content))
                    {
                        // Assert
                        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    }
                }
                finally
                {
                    Client.DefaultRequestHeaders.Remove("Authorization");
                }
            }
        }

        [Fact]
        public async Task DownloadNonExistedFileTest()
        {
            // Arrange
            using (var response = await Client.GetAsync("/api/File/" + default(Guid)))
            {
                // Assert #1
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

                var bytes = await response.Content.ReadAsByteArrayAsync();

                // Assert #2
                Assert.Empty(bytes);
            }
        }

        [Fact]
        public async Task DownloadExistedFileTest()
        {
            // Arrange
            Guid id = default;
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "base64", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAIAAACQd1PeAAAACXBIWXMAAAsTAAALEwEAmpwYAAAE82lUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS42LWMxNDIgNzkuMTYwOTI0LCAyMDE3LzA3LzEzLTAxOjA2OjM5ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0RXZ0PSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VFdmVudCMiIHhtbG5zOnBob3Rvc2hvcD0iaHR0cDovL25zLmFkb2JlLmNvbS9waG90b3Nob3AvMS4wLyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ0MgMjAxOCAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDE5LTA0LTE4VDE5OjU0OjAzKzA4OjAwIiB4bXA6TWV0YWRhdGFEYXRlPSIyMDE5LTA0LTE4VDE5OjU0OjAzKzA4OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAxOS0wNC0xOFQxOTo1NDowMyswODowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6MWY1ZDc0M2QtMGE1Ny02ODRkLWIxZmEtY2VjYWYxMmJlYWVkIiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjFmNWQ3NDNkLTBhNTctNjg0ZC1iMWZhLWNlY2FmMTJiZWFlZCIgeG1wTU06T3JpZ2luYWxEb2N1bWVudElEPSJ4bXAuZGlkOjFmNWQ3NDNkLTBhNTctNjg0ZC1iMWZhLWNlY2FmMTJiZWFlZCIgcGhvdG9zaG9wOkNvbG9yTW9kZT0iMyI+IDx4bXBNTTpIaXN0b3J5PiA8cmRmOlNlcT4gPHJkZjpsaSBzdEV2dDphY3Rpb249ImNyZWF0ZWQiIHN0RXZ0Omluc3RhbmNlSUQ9InhtcC5paWQ6MWY1ZDc0M2QtMGE1Ny02ODRkLWIxZmEtY2VjYWYxMmJlYWVkIiBzdEV2dDp3aGVuPSIyMDE5LTA0LTE4VDE5OjU0OjAzKzA4OjAwIiBzdEV2dDpzb2Z0d2FyZUFnZW50PSJBZG9iZSBQaG90b3Nob3AgQ0MgMjAxOCAoV2luZG93cykiLz4gPC9yZGY6U2VxPiA8L3htcE1NOkhpc3Rvcnk+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+Md1XMwAAAAxJREFUCJlj+M/AAAADAQEAnOO/WQAAAABJRU5ErkJggg==" }
            });
            using (var response = await Client.PostAsync("/api/File", content))
            {
                id = JsonConvert.DeserializeObject<Guid>(await response.Content.ReadAsStringAsync());
            }

            // Act
            using (var response = await Client.GetAsync("/api/File/" + id))
            {
                // Assert #1
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var bytes = await response.Content.ReadAsByteArrayAsync();

                // Assert #2
                Assert.NotEmpty(bytes);
            }
        }
    }
}
