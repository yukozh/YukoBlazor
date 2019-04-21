using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using YukoBlazor.Server.Models;
using YukoBlazor.Shared;

namespace YukoBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> Get(
            [FromServices] BlogContext db, Guid id, 
            CancellationToken token = default)
        {
            var config = JsonConvert.DeserializeObject<Config>(
                await System.IO.File.ReadAllTextAsync("config.json"));
            var comments = await db.Comments
                .Include(x => x.InnerComments)
                .Where(x => x.PostId == id)
                .OrderByDescending(x => x.Time)
                .Select(x => new CommentViewModel
                {
                    Id = x.Id,
                    AvatarUrl = GetAvatarUrl(x.IsOwner ? config.Profile.Email : x.Email),
                    Email = User.Identity.IsAuthenticated ? (x.IsOwner ? config.Profile.Email : x.Email) : null,
                    Content = x.Content,
                    Name = x.IsOwner ? config.Profile.Nickname : x.Name,
                    Time = x.Time,
                    IsOwner = x.IsOwner,
                    InnerComments = x.InnerComments.Select(y => new CommentViewModel
                    {
                        Id = y.Id,
                        AvatarUrl = GetAvatarUrl(y.IsOwner ? config.Profile.Email : y.Email),
                        Email = User.Identity.IsAuthenticated ? (y.IsOwner ? config.Profile.Email : y.Email) : null,
                        Content = y.Content,
                        Name = y.IsOwner ? config.Profile.Nickname : y.Name,
                        Time = y.Time,
                        IsOwner = y.IsOwner
                    })
                })
                .ToListAsync(token);
            return Json(comments);
        }

        [HttpPost("{id:Guid}")]
        public async Task<IActionResult> Post(
            [FromServices] BlogContext db, Guid id, string name, 
            string content, string email, bool isRootComment = true, 
            CancellationToken token = default)
        {
            var comment = new Comment();
            comment.Name = name;
            comment.Email = email;
            comment.Content = content;
            comment.IsOwner = User.Identity.IsAuthenticated;

            if (isRootComment)
            {
                if (!await db.Posts.AnyAsync(x => x.Id == id, token))
                {
                    Response.StatusCode = 404;
                    return Json("Post is not found");
                }

                comment.PostId = id;
            }
            else
            {
                if (!await db.Comments.AnyAsync(x => x.Id == id, token))
                {
                    Response.StatusCode = 404;
                    return Json("Parent comment is not found");
                }
                comment.ParentId = id;
            }

            db.Comments.Add(comment);
            await db.SaveChangesAsync(token);

            return Json(comment.Id);
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(
            [FromServices] BlogContext db, Guid id,
            CancellationToken token = default)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 403;
                return Json("Not Authroized");
            }

            var comment = await db.Comments
                .Include(x => x.InnerComments)
                .SingleOrDefaultAsync(x => x.Id == id, token);

            if (comment == null)
            {
                Response.StatusCode = 404;
                return Json($"The comment {id} is not found");
            }

            if (comment.InnerComments != null)
            {
                db.Comments.RemoveRange(comment);
            }
            db.Comments.Remove(comment);
            await db.SaveChangesAsync(token);
            return Json(true);
        }

        internal static string GetAvatarUrl(string email)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(email.Trim().ToLower());
                var hash = BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", string.Empty).ToLower();
                return $"//www.gravatar.com/avatar/{hash}?d=identicon";
            }
        }
    }
}
