using System.ComponentModel.DataAnnotations;

namespace CareFusion.Core.Entities;

/// <summary>
/// Represents a clinic site or location within the healthcare system
/// </summary>
public class ClinicSite : BaseEntity
{
    /// <summary>
    /// Name of the clinic site
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; } = default!;
    
    /// <summary>
    /// Code or identifier for the clinic site
    /// </summary>
    [MaxLength(50)]
    public string Code { get; set; } = default!;
    
    /// <summary>
    /// Street address of the clinic
    /// </summary>
    [MaxLength(200)]
    public string? Address { get; set; }
    
    /// <summary>
    /// City where the clinic is located
    /// </summary>
    [MaxLength(100)]
    public string? City { get; set; }
    
    /// <summary>
    /// State or province where the clinic is located
    /// </summary>
    [MaxLength(100)]
    public string? State { get; set; }
    
    /// <summary>
    /// Postal code for the clinic location
    /// </summary>
    [MaxLength(20)]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Country where the clinic is located
    /// </summary>
    [MaxLength(100)]
    public string? Country { get; set; }
    
    /// <summary>
    /// Primary phone number for the clinic
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }
    
    /// <summary>
    /// Email address for the clinic
    /// </summary>
    [MaxLength(256)]
    public string? Email { get; set; }
    
    /// <summary>
    /// Indicates whether this clinic site is currently active and operational
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Additional description or notes about the clinic site
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }
}