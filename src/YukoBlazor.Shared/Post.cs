using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukoBlazor.Shared
{
    public class Post
    {
        public Guid Id { get; set; }

        [MaxLength(256)]
        public string Url { get; set; }

        [MaxLength(256)]
        public string Title { get; set; }

        public string Summary { get; set; }

        public string Content { get; set; }

        public DateTime Time { get; set; } = DateTime.UtcNow;

        public bool IsPage { get; set; }

        public bool IsFeatured { get; set; }

        [ForeignKey("Catalog")]
        public string CatalogId { get; set; }

        public virtual Catalog Catalog { get; set; }

        public virtual ICollection<PostTag> Tags { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
