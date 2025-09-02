using CareFusion.Core.Configurations;
using CareFusion.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace CareFusion.Core;

/// <summary>
/// Database context for the CareFusion medical imaging system
/// Provides access to all entities and handles auditing, soft deletes, and change tracking
/// </summary>
public class CareFusionDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the CareFusionDbContext
    /// </summary>
    /// <param name="options">Database context options</param>
    public CareFusionDbContext(DbContextOptions<CareFusionDbContext> options) : base(options) { }

    /// <summary>
    /// DbSet for Patient entities - stores patient demographic and contact information
    /// </summary>
    public DbSet<Patient> Patients => Set<Patient>();
    
    /// <summary>
    /// DbSet for Exam entities - stores medical examination records
    /// </summary>
    public DbSet<Exam> Exams => Set<Exam>();
    
    /// <summary>
    /// DbSet for User entities - stores system user accounts and authentication information
    /// </summary>
    public DbSet<User> Users => Set<User>();
    
    /// <summary>
    /// DbSet for PatientNote entities - stores clinical notes for patients
    /// </summary>
    public DbSet<PatientNote> PatientNotes => Set<PatientNote>();
    
    /// <summary>
    /// DbSet for PatientReport entities - stores uploaded reports for patients
    /// </summary>
    public DbSet<PatientReport> PatientReports => Set<PatientReport>();
    
    /// <summary>
    /// DbSet for ExamImage entities - stores images associated with medical exams
    /// </summary>
    public DbSet<ExamImage> ExamImages => Set<ExamImage>();
    
    /// <summary>
    /// DbSet for ClinicSite entities - stores clinic site information
    /// </summary>
    public DbSet<ClinicSite> ClinicSites => Set<ClinicSite>();
    
    /// <summary>
    /// DbSet for AuditLog entities - stores audit trail of all database changes
    /// </summary>
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    /// <summary>
    /// Configures the database model including entity configurations, relationships, and query filters
    /// </summary>
    /// <param name="modelBuilder">Model builder for configuring the database schema</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new ExamConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new ClinicSiteConfiguration());
        
        // TODO: Create configurations for new entities
        // modelBuilder.ApplyConfiguration(new PatientNoteConfiguration());
        // modelBuilder.ApplyConfiguration(new PatientReportConfiguration());
        // modelBuilder.ApplyConfiguration(new ExamImageConfiguration());

        // Configure relationships for new entities manually until configurations are created
        ConfigurePatientNoteEntity(modelBuilder);
        ConfigurePatientReportEntity(modelBuilder);
        ConfigureExamImageEntity(modelBuilder);

        // Global query filter for soft delete (IsActive instead of IsDeleted)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entities.BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(CareFusionDbContext).GetMethod(nameof(ApplySoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, new object[] { modelBuilder });
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures the PatientNote entity relationships and constraints
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration</param>
    private static void ConfigurePatientNoteEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientNote>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NoteType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.AuthorName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NoteDate).IsRequired();
            
            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Notes)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.PatientId, e.NoteDate });
        });
    }

    /// <summary>
    /// Configures the PatientReport entity relationships and constraints
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration</param>
    private static void ConfigurePatientReportEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ReportType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FileSizeBytes).IsRequired();
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            
            entity.HasOne(e => e.Patient)
                  .WithMany(p => p.Reports)
                  .HasForeignKey(e => e.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.PatientId, e.ReportType });
        });
    }

    /// <summary>
    /// Configures the ExamImage entity relationships and constraints
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration</param>
    private static void ConfigureExamImageEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExamImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileSizeBytes).IsRequired();
            entity.Property(e => e.Width);
            entity.Property(e => e.Height);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.ThumbnailPath).HasMaxLength(500);
            
            entity.HasOne(e => e.Exam)
                  .WithMany(ex => ex.Images)
                  .HasForeignKey(e => e.ExamId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.ExamId);
        });
    }

    /// <summary>
    /// Applies soft delete query filter using IsActive property
    /// </summary>
    /// <typeparam name="TEntity">Entity type that inherits from BaseEntity</typeparam>
    /// <param name="modelBuilder">Model builder for applying the filter</param>
    private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : Entities.BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => SaveChangesAsync(user: null, cancellationToken);

    /// <summary>
    /// Saves changes to the database with audit trail support and automatic timestamp management
    /// Overload to pass the current username/email from the WebApi layer
    /// </summary>
    /// <param name="user">Username of the person making the changes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of entities affected</returns>
    public async Task<int> SaveChangesAsync(string? user, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Entities.BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.CreatedBy = user;
                entry.Entity.IsDeleted = false; // Ensure new entities are active
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAtUtc = utcNow;
                entry.Entity.ModifiedBy = user;
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Soft delete - mark as inactive instead of actually deleting
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.ModifiedAtUtc = utcNow;
                entry.Entity.ModifiedBy = user;
            }
        }

        // Build audit logs for tracking changes
        var auditLogs = BuildAuditLogs(ChangeTracker.Entries(), utcNow, user);
        if (auditLogs.Count > 0)
            await AuditLogs.AddRangeAsync(auditLogs, cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static List<AuditLog> BuildAuditLogs(IEnumerable<EntityEntry> entries, DateTime utcNow, string? user)
    {
        var logs = new List<AuditLog>();

        foreach (var e in entries.Where(x => x.Entity is BaseEntity))
        {
            string? action = e.State switch
            {
                EntityState.Added => "Insert",
                EntityState.Modified => "Update",
                EntityState.Deleted => "Delete", // won't be hit due to soft-delete conversion, but safe
                _ => null
            };

            if (action is null) continue;

            var entity = (BaseEntity)e.Entity;
            var changes = new Dictionary<string, object?>();

            if (e.State == EntityState.Modified)
            {
                foreach (var prop in e.Properties.Where(p => p.IsModified))
                {
                    changes[prop.Metadata.Name] = new
                    {
                        Old = prop.OriginalValue,
                        New = prop.CurrentValue
                    };
                }
            }

            logs.Add(new AuditLog
            {
                EntityName = e.Metadata.ClrType.Name,
                EntityId = entity.Id.ToString(),
                Action = action,
                TimestampUtc = utcNow,
                User = user,
                ChangesJson = changes.Count == 0 ? null : JsonSerializer.Serialize(changes)
            });
        }

        return logs;
    }
    
    /// <summary>
    /// Configures the ClinicSite entity relationships and constraints
    /// </summary>
    /// <param name="modelBuilder">Model builder for entity configuration</param>
    private static void ConfigureClinicSiteEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClinicSite>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Name);
        });
    }
}
