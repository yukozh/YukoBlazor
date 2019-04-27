using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YukoBlazor.Server.Models;
using YukoBlazor.Shared;

namespace YukoBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private const int PageSize = 10;

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] BlogContext db,
            string title, DateTime? from, DateTime? to, 
            string catalog, string tag, bool isPage = false,
            int page = 1, CancellationToken token = default)
        {
            IQueryable<Post> query = db.Posts
                .Include(x => x.Catalog)
                .Include(x => x.Tags)
                .Where(x => x.IsPage == isPage);

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(x => x.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(catalog))
            {
                query = query.Where(x => x.Catalog.Id == catalog);
            }

            if (!string.IsNullOrEmpty(tag))
            {
                query = query.Where(x => x.Tags.Any(y => y.Tag == tag));
            }

            if (from.HasValue)
            {
                query = query.Where(x => x.Time >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(x => x.Time < to.Value);
            }

            var dataCount = await query.CountAsync();
            var pageCount = (dataCount + PageSize - 1) / PageSize;
            var data = await query
                .OrderByDescending(x => x.IsFeatured)
                .ThenByDescending(x => x.Time)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(token);

            return Json(new PagedViewModel<PostViewModel>
            {
                CurrentPage = page,
                TotalCount = dataCount,
                TotalPage = pageCount,
                Data = data.Select(x => new PostViewModel
                {
                    Catalog = x.Catalog,
                    Id = x.Id,
                    Content = x.Summary,
                    Tags = x.Tags.Select(y => y.Tag),
                    Time = x.Time,
                    Title = x.Title,
                    Url = x.Url,
                    IsPage = x.IsPage,
                    IsFeatured = x.IsFeatured
                })
            });
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(
            [FromServices] BlogContext db, string id,
            CancellationToken token = default)
        {
            var post = await db.Posts
                .Include(x => x.Catalog)
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Url == id, token);

            if (post == null)
            {
                Response.StatusCode = 404;
                return Json(null);
            }
            else
            {
                return Json(new PostViewModel
                {
                    Id = post.Id,
                    Catalog = post.Catalog,
                    Content = post.Content,
                    Tags = post.Tags.Select(y => y.Tag),
                    Time = post.Time,
                    Title = post.Title,
                    Url = post.Url,
                    IsPage = post.IsPage,
                    IsFeatured = post.IsFeatured
                });
            }
        }

        [HttpPut("{url}")]
        [HttpPost("{url}")]
        public async Task<IActionResult> Put(
            [FromServices] BlogContext db, string url, 
            string catalog, string title, string tags, 
            string content, bool isPage = false, 
            bool isFeatured = false,
            CancellationToken token = default)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 403;
                return Json("Not Authorized");
            }

            if (await db.Posts.AnyAsync(x => x.Url == url, token))
            {
                Response.StatusCode = 400;
                return Json("URL is already exist");
            }

            if (!string.IsNullOrEmpty(catalog) 
                && !await db.Catalogs.AnyAsync(x => x.Id == catalog, token))
            {
                Response.StatusCode = 400;
                return Json("Catalog is not exist");
            }

            tags = tags ?? ""; // Defense null value of tags
            var tagsList = tags.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new PostTag
                {
                    Tag = x
                })
                .ToList();

            var post = new Post
            {
                Url = url,
                CatalogId = catalog,
                IsPage = isPage,
                IsFeatured = isFeatured,
                Tags = tagsList,
                Content = content,
                Title = title,
                Summary = TruncateContent(content)
            };

            db.Posts.Add(post);
            await db.SaveChangesAsync(token);
            return Json(post.Id);
        }

        [HttpPatch("{url}")]
        public async Task<IActionResult> Patch(
            [FromServices] BlogContext db, string content,
            string catalog, string title, string tags,
            string url, string newUrl, bool? isPage = null,
            bool? isFeatured = null, CancellationToken token = default)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 403;
                return Json("Not Authorized");
            }

            if (!string.IsNullOrWhiteSpace(url) 
                && await db.Posts.AnyAsync(x => x.Url == newUrl, token)
                && url != newUrl)
            {
                Response.StatusCode = 400;
                return Json("New URL is already exist");
            }

            if (!string.IsNullOrWhiteSpace(catalog) 
                && !await db.Catalogs.AnyAsync(x => x.Id == catalog, token))
            {
                Response.StatusCode = 400;
                return Json("Catalog is not exist");
            }

            var post = await db.Posts
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Url == url, token);

            if (post == null)
            {
                Response.StatusCode = 404;
                return Json("Post is not exist");
            }

            if (tags != null
                && post.Tags != null)
            {
                db.RemoveRange(post.Tags);
                var tagsList = tags.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => new PostTag
                    {
                        Tag = x
                    })
                    .ToList();
                post.Tags = tagsList;
            }

            if (!string.IsNullOrEmpty(newUrl))
            {
                post.Url = newUrl;
            }

            if (!string.IsNullOrEmpty(catalog))
            {
                post.CatalogId = catalog;
            }

            if (!string.IsNullOrEmpty(content))
            {
                post.Content = content;
                post.Summary = TruncateContent(content);
            }

            if (!string.IsNullOrEmpty(title))
            {
                post.Title = title;
            }

            if (isPage.HasValue)
            {
                post.IsPage = isPage.Value;
            }

            if (isFeatured.HasValue)
            {
                post.IsFeatured = isFeatured.Value;
            }

            await db.SaveChangesAsync(token);
            return Json(post.Id);
        }

        [HttpDelete("{url}")]
        public async Task<IActionResult> Delete(
            [FromServices] BlogContext db, string url,
            CancellationToken token = default)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 403;
                return Json("Not Authorized");
            }

            var post = await db.Posts
                .Include(x => x.Comments)
                .ThenInclude(x => x.InnerComments)
                .SingleOrDefaultAsync(x => x.Url == url, token);

            if (post == null)
            {
                Response.StatusCode = 404;
                return Json("Post is not exist");
            }

            if (post.Comments != null)
            {
                db.Comments.RemoveRange(post.Comments.SelectMany(x => x.InnerComments));
                db.Comments.RemoveRange(post.Comments);
            }
            db.Posts.Remove(post);
            await db.SaveChangesAsync(token);
            return Json(true);
        }

        internal static string TruncateContent(string content)
        {
            var summary = new StringBuilder();

            if (content != null)
            {
                var flag = false;
                var tmp = content
                    .Replace("\r", "")
                    .Split('\n');

                if (tmp.Count() > 10)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        if (tmp[i].StartsWith("```"))
                        {
                            flag = !flag;
                        }

                        summary.AppendLine(tmp[i]);
                    }
                    if (flag)
                    {
                        summary.AppendLine("```");
                    }
                }
                else
                {
                    return content;
                }
            }

            return summary.ToString();
        }
    }
}
