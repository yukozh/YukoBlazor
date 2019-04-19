using System;
using System.Collections.Generic;

namespace YukoBlazor.Shared
{
    public class CommentViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public DateTime Time { get; set; }

        public IEnumerable<CommentViewModel> InnerComments { get; set; }
    }
}
