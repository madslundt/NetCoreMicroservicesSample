using Microsoft.EntityFrameworkCore;

namespace DataModel.Models.Message
{
    public class MessageContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<Message>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.Property(p => p.UserId)
                    .IsRequired();

                b.Property(p => p.Text)
                    .IsRequired();

                b.Property(p => p.Created)
                    .IsRequired();

                b.HasKey(k => k.Id);
            });
        }
    }
}
