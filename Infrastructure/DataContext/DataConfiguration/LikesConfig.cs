using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataContext.DataConfiguration
{
    internal sealed class LikesConfig : IEntityTypeConfiguration<UserLikes>
    {
        public void Configure(EntityTypeBuilder<UserLikes> builder)
        {
            builder.HasKey(x => new { x.SourceUserId, x.TargetUserId });
        }
    }
}
