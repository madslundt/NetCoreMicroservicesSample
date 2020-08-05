using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Review
{
    public class ReviewContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<Review>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.Property(p => p.MovieId)
                    .IsRequired();

                b.Property(p => p.Text)
                    .IsRequired();

                b.Property(p => p.Rating)
                    .IsRequired();

                b.Property(p => p.CreatedUtc)
                    .IsRequired();

                b.HasOne(r => r.RatingRef)
                    .WithMany()
                    .HasForeignKey(fk => fk.Rating)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasIndex(i => i.MovieId);

                b.HasKey(k => k.Id);
            });
        }
    }
}
