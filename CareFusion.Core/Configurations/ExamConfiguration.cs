// Placeholder for Configurations/ExamConfiguration.cs
using CareFusion.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareFusion.Core.Configurations;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> b)
    {
        b.ToTable("Exams");
        b.HasKey(x => x.Id);

        b.Property(x => x.RowVersion)
            .IsRowVersion();

        b.Property(x => x.Modality).IsRequired().HasMaxLength(100);
        b.Property(x => x.StudyType).IsRequired().HasMaxLength(100);
        b.Property(x => x.StorageUri).HasMaxLength(500);
        b.Property(x => x.StorageKey).HasMaxLength(256);
        b.Property(x => x.Status).HasMaxLength(50).HasDefaultValue("New");

        b.HasIndex(x => new { x.PatientId, x.StudyDateUtc });
    }
}
