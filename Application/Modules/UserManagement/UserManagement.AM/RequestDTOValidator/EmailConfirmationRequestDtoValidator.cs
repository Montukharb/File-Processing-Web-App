using FluentValidation;
using UserManagement.AM.RequestDTO;

namespace UserManagement.AM.RequestDTOValidator;

public sealed class EmailConfirmationRequestDtoValidator : AbstractValidator<EmailConfirmationRequestDto>
{
    public EmailConfirmationRequestDtoValidator()
    {
        RuleFor(request => request.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(request => request.Token)
            .NotEmpty().WithMessage("Email confirmation token is required.");
    }
}