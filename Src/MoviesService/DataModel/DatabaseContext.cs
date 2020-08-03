using DataModel.Models;
using Microsoft.EntityFrameworkCore;

namespace DataModel
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            MovieContext.Build(builder);
        }
    }
}
