// Placeholder for Configurations/PatientConfiguration.cs
using CareFusion.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareFusion.Core.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> b)
    {
        b.ToTable("Patients");
        b.HasKey(x => x.Id);

        b.Property(x => x.RowVersion)
            .IsRowVersion();

        b.HasIndex(x => x.MRN)
            .IsUnique()
            .HasFilter("[MRN] IS NOT NULL");

        b.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        b.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        b.Property(x => x.MRN).HasMaxLength(50);
        b.Property(x => x.Gender).HasMaxLength(25);

        b.HasMany(x => x.Exams)
         .WithOne(x => x.Patient)
         .HasForeignKey(x => x.PatientId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
