using Microsoft.EntityFrameworkCore;
using OCTT.Domain.Entities;

namespace OCTT.Domain.Concrete
{
    public class RSSDbContext : DbContext
    {
        public DbSet<RSSRecord> RSSRecords { get; set; }

        public DbSet<RSSFeed> RSSFeeds { get; set; }

        public RSSDbContext(DbContextOptions<RSSDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            /// keys
            builder.Entity<RSSFeed>().HasIndex(p => new { p.Name, p.Uri }).IsUnique();
            builder.Entity<RSSRecord>().HasIndex(p => new { p.Title, p.PubDate }).IsUnique();
            /// one to many
            builder.Entity<RSSRecord>().HasOne(r => r.Feed).WithMany(f => f.Records).HasForeignKey(r => r.FeedId);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;");
            }
        }
    }
}
