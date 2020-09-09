using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EventStores.Stores.EfCore
{
    public class EfCoreEventStoreContext : DbContext
    {
        public EfCoreEventStoreContext(DbContextOptions<EfCoreEventStoreContext> options) : base(options)
        { }

        public DbSet<StreamState> Streams { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StreamState>(b =>
            {
                b.Property(p => p.Id)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.CreatedUtc)
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                b.Property(p => p.AggregateId)
                    .IsRequired();

                b.Property(p => p.Type)
                    .IsRequired();

                b.Property(p => p.Data)
                    .IsRequired();

                b.HasIndex(k => new { k.AggregateId, k.Version });

                b.HasKey(k => k.Id);
            });
        }
    }
}
