using UserManagement.AM.RequestDTO;
using UserManagement.AM.RequestDTOValidator;

namespace FileProcessing.Tests.Unit.Modules.UserManagement.Validators;

public sealed class EmailConfirmationRequestDtoValidatorTests
{
    private readonly EmailConfirmationRequestDtoValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidRequest_IsValid()
    {
        var request = new EmailConfirmationRequestDto
        {
            UserId = "user-id",
            Token = "confirmation-token"
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_MissingToken_IsInvalid()
    {
        var request = new EmailConfirmationRequestDto
        {
            UserId = "user-id",
            Token = string.Empty
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Token));
    }
}