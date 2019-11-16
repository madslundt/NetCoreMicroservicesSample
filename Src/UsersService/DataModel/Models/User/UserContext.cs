using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.User
{
    public class UserContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<User>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.Property(p => p.FirstName)
                    .IsRequired();

                b.Property(p => p.LastName)
                    .IsRequired();

                b.Property(p => p.Email)
                    .IsRequired();

                b.HasIndex(k => k.Email)
                    .IsUnique();

                b.HasOne(r => r.UserStatusRef)
                    .WithMany()
                    .HasForeignKey(fk => fk.Status)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

                b.HasKey(k => k.Id);
            });
        }
    }
}
