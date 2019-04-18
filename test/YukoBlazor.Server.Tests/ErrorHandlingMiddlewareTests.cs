using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;
using Moq;

namespace YukoBlazor.Server.Tests
{
    public class ErrorHandlingMiddlewareTests
    {
        [Fact]
        public async Task ExceptionTests()
        {
            // Arrange
            var statusCode = 200;
            var contentType = "";
            var stream = new MemoryStream();
            var fakeResponse = new Mock<HttpResponse>();
            fakeResponse
                .SetupSet(x => x.StatusCode)
                .Callback(val => statusCode = val);
            fakeResponse
                .SetupSet(x => x.ContentType)
                .Callback(val => contentType = val);
            fakeResponse
                .SetupGet(x => x.BodyPipe)
                .Returns(new StreamPipeWriter(stream));
            fakeResponse
                .SetupGet(x => x.Body)
                .Returns(stream);
            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext
                .SetupGet(x => x.Response)
                .Returns(fakeResponse.Object);
            var handler = new ErrorHandlingMiddleware((ctx => 
            {
                throw new Exception("Test Exception");
            }));

            // Act
            await handler.Invoke(fakeHttpContext.Object);

            // Assert
            Assert.Equal(500, statusCode);
            Assert.Equal("application/json", contentType);
        }
    }
}
