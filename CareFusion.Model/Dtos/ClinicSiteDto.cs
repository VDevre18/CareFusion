namespace CareFusion.Model.Dtos;

/// <summary>
/// Data transfer object for clinic site information
/// </summary>
public class ClinicSiteDto
{
    /// <summary>
    /// Unique identifier for the clinic site
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Name of the clinic site
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Code or identifier for the clinic site
    /// </summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>
    /// Street address of the clinic
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// City where the clinic is located
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// State or province where the clinic is located
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Postal code for the clinic location
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Country where the clinic is located
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Primary phone number for the clinic
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Email address for the clinic
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Indicates whether this clinic site is currently active and operational
    /// </summary>
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Additional description or notes about the clinic site
    /// </summary>
    public string? Description { get; set; }
}