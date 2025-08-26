// Placeholder for CareFusionDbContext.cs
using CareFusion.Core.Configurations;
using CareFusion.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace CareFusion.Core;

public class CareFusionDbContext : DbContext
{
    public CareFusionDbContext(DbContextOptions<CareFusionDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PatientConfiguration());
        modelBuilder.ApplyConfiguration(new ExamConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());

        // Global query filter for soft delete
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

    private static void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : Entities.BaseEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => SaveChangesAsync(user: null, cancellationToken);

    /// <summary>
    /// Overload to pass the current username/email from the WebApi layer.
    /// </summary>
    public async Task<int> SaveChangesAsync(string? user, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Entities.BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.CreatedBy = user;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAtUtc = utcNow;
                entry.Entity.ModifiedBy = user;
            }
            else if (entry.State == EntityState.Deleted)
            {
                // Soft delete
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.ModifiedAtUtc = utcNow;
                entry.Entity.ModifiedBy = user;
            }
        }

        // Optional: basic auditing (entity, key, action).
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
                EntityId = entity.Id,
                Action = action,
                TimestampUtc = utcNow,
                User = user,
                ChangesJson = changes.Count == 0 ? null : JsonSerializer.Serialize(changes)
            });
        }

        return logs;
    }
}
