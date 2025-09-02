namespace CareFusion.Model.Dtos;

/// <summary>
/// Data transfer object for user information
/// </summary>
public class UserDto
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Username for login
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's role in the system
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// User's phone number
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// User's department
    /// </summary>
    public string? Department { get; set; }
    
    /// <summary>
    /// Password for new users or password changes
    /// </summary>
    public string? Password { get; set; }
    
    /// <summary>
    /// Default clinic site ID for the user
    /// </summary>
    public int? DefaultClinicSiteId { get; set; }
}
