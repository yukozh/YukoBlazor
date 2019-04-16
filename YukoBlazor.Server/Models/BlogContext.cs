using Microsoft.EntityFrameworkCore;
using YukoBlazor.Shared;

namespace YukoBlazor.Server.Models
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> opt)
            : base(opt)
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostTag> PostTags { get; set; }

        public DbSet<Catalog> Catalogs { get; set; }

        public DbSet<Blob> Blobs { get; set; }

        public DbSet<BlogRoll> BlogRolls { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Catalog>(e =>
            {
                e.HasIndex(x => x.Priority);
            });

            builder.Entity<Comment>(e =>
            {
                e.HasIndex(x => x.Time);
            });

            builder.Entity<Post>(e =>
            {
                e.HasIndex(x => x.IsPage);
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.Url).IsUnique();
            });

            builder.Entity<PostTag>(e =>
            {
                e.HasKey(x => new { x.PostId, x.Tag });
            });

            builder.Entity<BlogRoll>(e =>
            {
                e.HasIndex(x => x.GitHubId);
            });
        }
    }
}
