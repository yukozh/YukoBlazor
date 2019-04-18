using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YukoBlazor.Server.Models;
using YukoBlazor.Shared;

namespace YukoBlazor.Server.Controllers
{
    public class ManageController : Controller
    {
        #region Catalog CRUD
        [HttpPut("api/Catalog/{url}")]
        [HttpPost("api/Catalog/{url}")]
        public async Task<IActionResult> PutCatalog(
            [FromServices] BlogContext db, 
            string url, string display, int priority,
            CancellationToken token = default)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 403;
                return Json("Not Authroized");
            }

            if (string.IsNullOrWhiteSpace(url)
                || string.IsNullOrWhiteSpace(display))
            {
                Response.StatusCode = 400;
                return Json("Url or Display could not be null or whitespace");
            }

            if (await db.Catalogs.AnyAsync(x => x.Id == url, token))
            {
                Response.StatusCode = 400;
                return Json($"The URL {url} is already exist");
            }

            db.Catalogs.Add(new Catalog
            {
                Id = url,
                Priority = priority,
                Display = display
            });
            await db.SaveChangesAsync(token);

            return Json(true);
        }

        [HttpPatch("api/Catalog/{url}")]
        public async Task<IActionResult> PatchCatalog(
            [FromServices] BlogContext db, string url, 
            string display, int? priority,
            CancellationToken token = default)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 403;
                return Json("Not Authroized");
            }

            var catalog = await db.Catalogs
                .Include(x => x.Posts)
                .SingleOrDefaultAsync(x => x.Id == url, token);

            if (catalog == null)
            {
                Response.StatusCode = 404;
                return Json($"The URL {url} is not found");
            }

            if (!string.IsNullOrEmpty(display))
            {
                catalog.Display = display;
            }

            if (priority.HasValue)
            {
                catalog.Priority = priority.Value;
            }

            await db.SaveChangesAsync(token);
            return Json(true);
        }

        [HttpDelete("api/Catalog/{url}")]
        public async Task<IActionResult> DeleteCatalog(
            [FromServices] BlogContext db, string url,
            CancellationToken token = default)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 403;
                return Json("Not Authroized");
            }

            var catalog = await db.Catalogs
                .Include(x => x.Posts)
                .SingleOrDefaultAsync(x => x.Id == url, token);

            if (catalog == null)
            {
                Response.StatusCode = 404;
                return Json($"The URL {url} is not found");
            }

            // Set null manually
            foreach(var x in catalog.Posts)
            {
                x.CatalogId = null;
            }

            db.Catalogs.Remove(catalog);
            await db.SaveChangesAsync(token);
            return Json(true);
        }
        #endregion
    }
}
