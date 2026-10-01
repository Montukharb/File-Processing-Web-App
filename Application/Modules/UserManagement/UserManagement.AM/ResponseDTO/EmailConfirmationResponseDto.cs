namespace UserManagement.AM.ResponseDTO;

public sealed class EmailConfirmationResponseDto
{
    public bool Success { get; init; }
    public required string Message { get; init; }
    public string? Email { get; init; }
}