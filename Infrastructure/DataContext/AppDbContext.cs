using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.DataContext
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser,IdentityRole<Guid>,Guid>(options)
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLikes> Likes { get; set; }
        public DbSet<Message> Message { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                );

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
               v => v.HasValue ? v.Value.ToUniversalTime() : null,
               v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
               );
            foreach (var entiryType in modelBuilder.Model.GetEntityTypes() )
            {
                foreach(var property in entiryType.GetProperties())
                {
                    if(property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if(property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }



            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        }
    }
}
