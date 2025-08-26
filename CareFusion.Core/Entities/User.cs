// Placeholder for Entities/User.cs
using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

public class User : BaseEntity
{
    [MaxLength(100)]
    public string Username { get; set; } = default!;
    [MaxLength(256)]
    public string Email { get; set; } = default!;
    [MaxLength(256)]
    public string PasswordHash { get; set; } = default!;

    [MaxLength(50)]
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;
}
