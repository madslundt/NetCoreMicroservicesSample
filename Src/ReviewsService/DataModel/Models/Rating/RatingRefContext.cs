using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DataModel.Models.Rating
{
    public class RatingRefContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<RatingRef>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.HasKey(k => k.Id);

                b.HasData(Enum.GetValues(typeof(RatingEnum)).Cast<RatingEnum>().Select(rating => new RatingRef(rating)).ToArray());
            });
        }
    }
}
