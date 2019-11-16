using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DataModel.Models.UserStatus
{
    public class UserStatusRefContext
    {
        public static void Build(ModelBuilder builder)
        {
            builder.Entity<UserStatusRef>(b =>
            {
                b.Property(p => p.Id)
                    .IsRequired();

                b.HasKey(k => k.Id);

                b.HasData(Enum.GetValues(typeof(UserStatusEnum)).Cast<UserStatusEnum>().Select(userStatus => new UserStatusRef(userStatus)).ToArray());
            });
        }
    }
}
