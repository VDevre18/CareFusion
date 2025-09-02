using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

/// <summary>
/// Represents a system user with authentication and authorization information
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Unique username for the user (used for login)
    /// </summary>
    [MaxLength(100)]
    public string Username { get; set; } = default!;
    
    /// <summary>
    /// Email address of the user
    /// </summary>
    [MaxLength(256)]
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// Hashed password for authentication
    /// </summary>
    [MaxLength(256)]
    public string PasswordHash { get; set; } = default!;

    /// <summary>
    /// User's role in the system (Admin, Doctor, Technician, etc.)
    /// </summary>
    [MaxLength(50)]
    public string Role { get; set; } = "User";

    /// <summary>
    /// Indicates whether the user account is active and can be used for login
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// User's first name
    /// </summary>
    [MaxLength(100)]
    public string? FirstName { get; set; }
    
    /// <summary>
    /// User's last name  
    /// </summary>
    [MaxLength(100)]
    public string? LastName { get; set; }
    
    /// <summary>
    /// Default clinic site ID for the user (determines which patients they can see)
    /// </summary>
    public int? DefaultClinicSiteId { get; set; }
    
    /// <summary>
    /// Navigation property to the default clinic site
    /// </summary>
    public ClinicSite? DefaultClinicSite { get; set; }
}
