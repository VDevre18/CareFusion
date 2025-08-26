using CareFusion.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareFusion.Core.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("AuditLogs");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd(); // identity
        b.Property(x => x.EntityName).IsRequired().HasMaxLength(100);
        b.Property(x => x.Action).IsRequired().HasMaxLength(50);
        b.Property(x => x.User).HasMaxLength(256);
        b.HasIndex(x => new { x.EntityName, x.EntityId, x.TimestampUtc });
    }
}
