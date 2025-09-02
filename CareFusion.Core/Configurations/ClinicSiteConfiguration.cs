using CareFusion.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareFusion.Core.Configurations;

/// <summary>
/// Entity Framework configuration for the ClinicSite entity
/// Defines database mappings, constraints, and relationships
/// </summary>
public class ClinicSiteConfiguration : IEntityTypeConfiguration<ClinicSite>
{
    /// <summary>
    /// Configures the ClinicSite entity mappings
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<ClinicSite> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        // Required fields with length constraints
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50);

        // Address fields
        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.City)
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .HasMaxLength(50);

        builder.Property(e => e.PostalCode)
            .HasMaxLength(20);

        builder.Property(e => e.Country)
            .HasMaxLength(100);

        // Contact information
        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.Email)
            .HasMaxLength(255);

        // Description
        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        // Audit fields
        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(255);

        builder.Property(e => e.ModifiedAtUtc);

        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(255);

        // Soft delete
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Row version for concurrency
        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

        // Indexes
        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("IX_ClinicSites_Code");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_ClinicSites_Name");

        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName("IX_ClinicSites_IsDeleted");

        // Global query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);

        // Table name
        builder.ToTable("ClinicSites");
    }
}