using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Configurations;

public class AppUserConfiguration  : IEntityTypeConfiguration<AppUser>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.Age)
            .HasMaxLength(200);
    }
}
