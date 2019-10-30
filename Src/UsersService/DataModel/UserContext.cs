using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataModel
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

                b.Property(p => p.Created)
                    .IsRequired();

                b.HasKey(k => k.Id);
            });
        }
    }
}
