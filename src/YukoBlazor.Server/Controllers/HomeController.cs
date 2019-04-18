using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YukoBlazor.Server.Models;
using YukoBlazor.Shared;

namespace YukoBlazor.Server.Controllers
{
    public class HomeController : Controller
    {
        internal const string ServiceOkText = "API Server is running";
        internal const string Authenticated = "Authenticated";
        internal const string NotAuthenticated = "Not Authenticated";

        [HttpGet("api/Hello")]
        public IActionResult Index()
        {
            return Content(ServiceOkText);
        }

        [HttpGet("api/State")]
        public IActionResult State()
        {
            return User.Identity.IsAuthenticated 
                ? Content(Authenticated) 
                : Content(NotAuthenticated);
        }

        [HttpGet("api/Tag")]
        public async Task<IActionResult> GetTag(
            [FromServices] BlogContext db, 
            CancellationToken token = default)
        {
            var groupByTags = await db.PostTags
                .OrderBy(x => x.Tag)
                .GroupBy(x => x.Tag)
                .Select(x => new TagViewModel
                {
                    Title = x.Key,
                    Count = x.Count()
                })
                .ToListAsync(token);

            return Json(groupByTags);
        }

        [HttpGet("api/Calendar")]
        public async Task<IActionResult> GetCalendar(
            [FromServices] BlogContext db,
            CancellationToken token = default)
        {
            var groupByCalendar = await db.Posts
                .Where(x => !x.IsPage)
                .OrderByDescending(x => x.Time)
                .GroupBy(x => new { x.Time.Year, x.Time.Month })
                .Select(x => new CalendarViewModel
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    Count = x.Count()
                })
                .ToListAsync(token);

            return Json(groupByCalendar);
        }

        [HttpGet("api/Catalog")]
        public async Task<IActionResult> GetCatalog(
            [FromServices] BlogContext db,
            CancellationToken token = default)
        {
            var groupByCatalog = await db.Catalogs
                .Include(x => x.Posts)
                .OrderByDescending(x => x.Priority)
                .Select(x => new CatalogViewModel
                {
                    Id = x.Id,
                    Display = x.Display,
                    Count = x.Posts.Count(),
                    Priority = x.Priority
                })
                .ToListAsync(token);

            return Json(groupByCatalog);
        }
    }
}
