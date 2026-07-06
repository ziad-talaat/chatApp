using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataContext.DataConfiguration
{
    internal sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Gender).IsRequired(true);
            builder.Property(x => x.City).IsRequired(true);
            builder.Property(x => x.Country).IsRequired(true);

            builder.HasMany(x => x.Photos).WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.HasMany(x => x.UsersWhoILike)
                .WithOne(x => x.SourceUser)
                .HasForeignKey(x => x.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x =>x.UsersWhoLikeMe)
                .WithOne(x => x.TargetUser)
                .HasForeignKey(x => x.TargetUserId)
                .OnDelete(DeleteBehavior.NoAction);




            builder.HasMany(x=>x.SendMessages)
                .WithOne(x=>x.Sender)
                .HasForeignKey(x=>x.SenderId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(x => x.RecipientMessages)
               .WithOne(x => x.Recipient)
               .HasForeignKey(x => x.RecipientId)
               .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
