using Microsoft.EntityFrameworkCore;

namespace DataModel.Models
{
    public class MovieContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<Movie>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.Property(p => p.Title)
                    .IsRequired();

                b.Property(p => p.Year)
                    .IsRequired();

                b.HasKey(k => k.Id);
            });
        }
    }
}
