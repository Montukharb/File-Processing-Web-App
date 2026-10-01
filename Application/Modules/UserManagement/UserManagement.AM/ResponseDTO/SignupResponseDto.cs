namespace UserManagement.AM.ResponseDTO
{
    public sealed class SignupResponseDto
    {
        public bool Success { get; init; }
        public required string Message { get; init; }
        public string? UserId { get; init; }
        public string? UserName { get; init; }
        public string? Email { get; init; }
    }
}
