using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukoBlazor.Shared
{
    public class Comment
    {
        public Guid Id { get; set; }

        [ForeignKey("Post")]
        public Guid? PostId { get; set; }

        public virtual Post Post { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        [MaxLength(32)]
        public string Email { get; set; }

        public bool IsOwner { get; set; }

        public string Content { get; set; }

        [ForeignKey("Parent")]
        public Guid? ParentId { get; set; }

        public virtual Comment Parent { get; set; }

        public DateTime Time { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Comment> InnerComments { get; set; }
    }
}
