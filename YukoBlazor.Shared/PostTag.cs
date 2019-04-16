using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukoBlazor.Shared
{
    public class PostTag
    {
        [ForeignKey("Post")]
        public Guid PostId { get; set; }

        public virtual Post Post { get; set; }

        [MaxLength(64)]
        public string Tag { get; set; }
    }
}
