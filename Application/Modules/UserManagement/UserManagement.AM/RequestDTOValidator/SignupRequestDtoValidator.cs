using FluentValidation;
using UserManagement.AM.RequestDTO;

namespace UserManagement.AM.RequestDTOValidator
{
    public sealed class SignupRequestDtoValidator : AbstractValidator<SignupRequestDto>
    {
        public SignupRequestDtoValidator()
        {
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(254).WithMessage("Email cannot exceed 254 characters.")
                .EmailAddress().WithMessage("Enter a valid email address.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .MaximumLength(128).WithMessage("Password cannot exceed 128 characters.")
                .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                .Matches("\\d").WithMessage("Password must contain a number.")
                .Matches("[^A-Za-z0-9]").WithMessage("Password must contain a special character.");

            RuleFor(x => x.ConfirmPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password confirmation is required.")
                .Equal(x => x.Password).WithMessage("Password confirmation must match the password.");
        }
    }
}
