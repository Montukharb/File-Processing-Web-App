using FluentValidation;
using UserManagement.AM.ResponseDTO;

namespace UserManagement.AM.ResponseDTOValidator;

public sealed class ProfileResponseDtoValidator : AbstractValidator<ProfileResponseDto>
{
    public ProfileResponseDtoValidator()
    {
        RuleFor(response => response.UserId)
            .NotEmpty().WithMessage("A user ID is required.");

        RuleFor(response => response.UserName)
            .NotEmpty().WithMessage("A username is required.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

        RuleFor(response => response.Email)
            .EmailAddress()
            .When(response => !string.IsNullOrWhiteSpace(response.Email));

        RuleFor(response => response.PhoneNumber)
            .MaximumLength(30)
            .When(response => response.PhoneNumber is not null);
    }
}