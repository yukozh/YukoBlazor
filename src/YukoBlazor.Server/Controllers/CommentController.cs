using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var comments = await db.Comments
                .Include(x => x.InnerComments)
                .Where(x => x.PostId == id)
                .OrderByDescending(x => x.Time)
                .Select(x => new CommentViewModel
                {
                    Id = x.Id,
                    AvatarUrl = null, // TODO: Onboard Gravatar
                    Email = User.Identity.IsAuthenticated ? x.Email : null,
                    Content = x.Content,
                    Name = x.Name,
                    Time = x.Time,
                    InnerComments = x.InnerComments.Select(y => new CommentViewModel
                    {
                        Id = y.Id,
                        AvatarUrl = null, // TODO: Onboard Gravatar
                        Email = User.Identity.IsAuthenticated ? y.Email : null,
                        Content = y.Content,
                        Name = y.Name,
                        Time = y.Time
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
                .SingleOrDefaultAsync(x => x.Id == id, token);

            if (comment == null)
            {
                Response.StatusCode = 404;
                return Json($"The comment {id} is not found");
            }

            db.Comments.Remove(comment);
            await db.SaveChangesAsync(token);
            return Json(true);
        }
    }
}
