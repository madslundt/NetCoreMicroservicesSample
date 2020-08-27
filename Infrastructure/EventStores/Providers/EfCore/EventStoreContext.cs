using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EventStores.Providers.EfCore
{
    public class EventStoreContext : DbContext
    {
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

                b.HasIndex(k => new { k.AggregateId, k.Version });

                b.HasKey(k => k.Id);
            });
        }
    }
}
