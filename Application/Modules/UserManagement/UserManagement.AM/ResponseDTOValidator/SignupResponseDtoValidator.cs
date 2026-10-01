using FluentValidation;
using UserManagement.AM.ResponseDTO;

namespace UserManagement.AM.ResponseDTOValidator
{
    public sealed class SignupResponseDtoValidator : AbstractValidator<SignupResponseDto>
    {
        public SignupResponseDtoValidator()
        {
            RuleFor(response => response.Message)
                .NotEmpty().WithMessage("A signup response message is required.")
                .MaximumLength(500).WithMessage("The signup response message cannot exceed 500 characters.");

            When(response => response.Success, () =>
            {
                RuleFor(response => response.UserId)
                    .NotEmpty().WithMessage("A user ID is required for a successful signup.");

                RuleFor(response => response.UserName)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("A username is required for a successful signup.")
                    .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
                    .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");

                RuleFor(response => response.Email)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("An email address is required for a successful signup.")
                    .MaximumLength(254).WithMessage("Email cannot exceed 254 characters.")
                    .EmailAddress().WithMessage("Enter a valid email address.");
            });
        }
    }
}