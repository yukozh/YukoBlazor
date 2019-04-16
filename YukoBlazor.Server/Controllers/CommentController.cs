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
    }
}
