using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YukoBlazor.Server.Models;
using YukoBlazor.Shared;

namespace YukoBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    public class FileController : Controller
    {
        internal const int GuestLimit = 1024 * 512;

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> Get(
            [FromServices] BlogContext db, Guid id,
            CancellationToken token = default)
        {
            var file = await db.Blobs
                .SingleOrDefaultAsync(x => x.Id == id, token);

            if (file == null)
            {
                Response.StatusCode = 404;
                return File(new byte[] { }, "application/octet-stream");
            }

            return File(file.Bytes, file.ContentType, file.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromServices] BlogContext db,
            IFormFile file, string base64,
            CancellationToken token = default)
        {
            if (file != null)
            {
                if (!User.Identity.IsAuthenticated && file.Length > GuestLimit)
                {
                    Response.StatusCode = 400;
                    return Json("File is too large");
                }

                byte[] bytes = new byte[file.Length];
                using (var reader = new BinaryReader(file.OpenReadStream()))
                {
                    bytes = reader.ReadBytes(Convert.ToInt32(file.Length));
                }

                var blob = new Blob
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    ContentLength = file.Length,
                    Time = DateTime.UtcNow,
                    Bytes = bytes
                };

                db.Blobs.Add(blob);
                await db.SaveChangesAsync(token);

                return Json(blob.Id);
            }
            else if (base64 != null)
            {
                // TODO: Validate base 64 string format and return 4xx to client if it is invalid.

                if (!User.Identity.IsAuthenticated && base64.Length > GuestLimit)
                {
                    Response.StatusCode = 400;
                    return Json("File is too large");
                }

                var contentString = base64.Split(',')[1].Trim();
                var contentType = base64.Split(':')[1].Split(';')[0];
                var bytes = Convert.FromBase64String(contentString);

                var blob = new Blob
                {
                    FileName = "file",
                    ContentType = contentType,
                    ContentLength = bytes.Length,
                    Time = DateTime.UtcNow,
                    Bytes = bytes
                };

                db.Blobs.Add(blob);
                await db.SaveChangesAsync(token);

                return Json(blob.Id);
            }
            else
            {
                Response.StatusCode = 400;
                return Json("Input format is incorrect");
            }
        }
    }
}
