using DataModel.Models.User;
using DataModel.Models.UserStatus;
using Microsoft.EntityFrameworkCore;

namespace DataModel
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {}

        public DbSet<User> Users { get; set; }
        public DbSet<UserStatusRef> UserStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            UserContext.Build(builder);
            UserStatusRefContext.Build(builder);
        }
    }
}
