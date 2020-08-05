using DataModel.Models.Rating;
using DataModel.Models.Review;
using Microsoft.EntityFrameworkCore;

namespace DataModel
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<RatingRef> RatingRefs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ReviewContext.Build(builder);
            RatingRefContext.Build(builder);
        }
    }
}
