using UserManagement.AM.RequestDTO;
using UserManagement.AM.RequestDTOValidator;

namespace FileProcessing.Tests.Unit.Modules.UserManagement.Validators;

public sealed class SignupRequestDtoValidatorTests
{
    private readonly SignupRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidRequest_IsValid()
    {
        var request = new SignupRequestDto
        {
            UserName = "valid-user",
            Email = "user@example.com",
            Password = "StrongPass123!",
            ConfirmPassword = "StrongPass123!"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_InvalidEmailAndPassword_ReturnsPropertyErrors()
    {
        var request = new SignupRequestDto
        {
            UserName = "u",
            Email = "invalid-email",
            Password = "weak",
            ConfirmPassword = "different"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Password));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.ConfirmPassword));
    }
}