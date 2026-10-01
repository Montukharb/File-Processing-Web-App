using UserManagement.AM.ResponseDTO;
using UserManagement.AM.ResponseDTOValidator;

namespace FileProcessing.Tests.Unit.Modules.UserManagement.Validators;

public sealed class ProfileResponseDtoValidatorTests
{
    private readonly ProfileResponseDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidProfile_IsValid()
    {
        var response = new ProfileResponseDto
        {
            UserId = "user-id",
            UserName = "valid-user",
            Email = "user@example.com",
            EmailConfirmed = true,
            PhoneNumber = "+12025550123"
        };

        var result = await _validator.ValidateAsync(response);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_InvalidEmail_IsInvalid()
    {
        var response = new ProfileResponseDto
        {
            UserId = "user-id",
            UserName = "valid-user",
            Email = "invalid-email",
            EmailConfirmed = false
        };

        var result = await _validator.ValidateAsync(response);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(response.Email));
    }
}