using UserManagement.AM.RequestDTO;
using UserManagement.AM.RequestDTOValidator;

namespace FileProcessing.Tests.Unit.Modules.UserManagement.Validators;

public sealed class LoginRequestDtoValidatorTests
{
    private readonly LoginRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidRequest_IsValid()
    {
        var request = new LoginRequestDto
        {
            Email = "user@example.com",
            Password = "password"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_MissingEmailAndPassword_IsInvalid()
    {
        var request = new LoginRequestDto
        {
            Email = string.Empty,
            Password = string.Empty
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Password));
    }
}