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
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(
            [FromServices] BlogContext db, Guid id, 
            CancellationToken token = default)
        {
            var comments = await db.Comments
                .Include(x => x.InnerComments)
                .Where(x => x.PostId == id)
                .OrderByDescending(x => x.Time)
                .ToListAsync(token);

            return Json(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromServices] BlogContext db, Guid id,
            string content, bool isRootComment = true,
            CancellationToken token = default)
        {
            var comment = new Comment();
            comment.Content = content;
            comment.IsOwner = User.Identity.IsAuthenticated;

            if (isRootComment)
            {
                comment.PostId = id;
            }
            else
            {
                comment.ParentId = id;
            }

            db.Comments.Add(comment);
            await db.SaveChangesAsync(token);

            return Json(true);
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
