// Placeholder for Dtos/UserDto.cs
namespace CareFusion.Model.Dtos;

public record UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
    public bool IsActive { get; init; }
}
