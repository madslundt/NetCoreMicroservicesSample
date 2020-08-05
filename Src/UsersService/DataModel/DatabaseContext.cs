using DataModel.Models.User;
using DataModel.Models.UserStatus;
using Infrastructure.Core.Events;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DataModel
{
    public class DatabaseContext : DbContext
    {
        private readonly IEventBus _eventBus;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, IEventBus eventBus = null) : base(options)
        {
            _eventBus = eventBus;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserStatusRef> UserStatusRefs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            UserContext.Build(builder);
            UserStatusRefContext.Build(builder);
        }

        public async Task SaveChangesAndCommit(IEvent @event)
        {
            using (var transaction = Database.BeginTransaction())
            {
                await SaveChangesAsync();
                await _eventBus.Commit(@event);

                await transaction.CommitAsync();
            }
        }
    }
}
