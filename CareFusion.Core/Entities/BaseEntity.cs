namespace CareFusion.Core.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Auditing (set in DbContext SaveChanges)
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public string? ModifiedBy { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }

    // Concurrency
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
