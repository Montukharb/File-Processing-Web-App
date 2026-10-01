namespace UserManagement.AM.ResponseDTO;

public sealed class ProfileResponseDto
{
    public required string UserId { get; init; }
    public required string UserName { get; init; }
    public string? Email { get; init; }
    public bool EmailConfirmed { get; init; }
    public string? PhoneNumber { get; init; }
}