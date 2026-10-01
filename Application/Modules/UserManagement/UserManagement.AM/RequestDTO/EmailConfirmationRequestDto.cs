namespace UserManagement.AM.RequestDTO;

public sealed class EmailConfirmationRequestDto
{
    public string UserId { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}