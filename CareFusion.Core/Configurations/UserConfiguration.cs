// Placeholder for Configurations/UserConfiguration.cs
using CareFusion.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareFusion.Core.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasKey(x => x.Id);

        b.Property(x => x.RowVersion)
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();

        b.HasIndex(x => x.Username).IsUnique();
        b.HasIndex(x => x.Email).IsUnique();

        b.Property(x => x.Username).IsRequired().HasMaxLength(100);
        b.Property(x => x.Email).IsRequired().HasMaxLength(256);
        b.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
        b.Property(x => x.Role).IsRequired().HasMaxLength(50);
    }
}
