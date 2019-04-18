using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;

namespace YukoBlazor.Server.Tests
{
    public class FileUploadTests : WebApiTestBase
    {
        [Fact]
        public async Task GuestUploadSuccessTest()
        {
            var blob = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new ByteArrayContent(blob), "file", "file.bin");
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
                    Assert.Null(exception);
                }
            }
        }
    }
}
