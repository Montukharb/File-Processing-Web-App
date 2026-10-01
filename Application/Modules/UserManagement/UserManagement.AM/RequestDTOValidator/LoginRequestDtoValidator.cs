using FluentValidation;
using UserManagement.AM.RequestDTO;

namespace UserManagement.AM.RequestDTOValidator
{
    public sealed class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestDtoValidator()
        {
            RuleFor(request => request.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(254).WithMessage("Email cannot exceed 254 characters.")
                .EmailAddress().WithMessage("Enter a valid email address.");

            RuleFor(request => request.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(128).WithMessage("Password cannot exceed 128 characters.");
        }
    }
}