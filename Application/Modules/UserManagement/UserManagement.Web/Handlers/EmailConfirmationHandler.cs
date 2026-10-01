using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using UserManagement.AM.RequestDTO;
using UserManagement.AM.ResponseDTO;

namespace UserManagement.Web.Handlers;

public static class EmailConfirmationHandler
{
    public static async Task<IResult> HandleAsync(
        [AsParameters] EmailConfirmationRequestDto request,
        IValidator<EmailConfirmationRequestDto> validator,
        UserManager<ApplicationUser> userManager,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

            return Results.ValidationProblem(errors);
        }

        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null)
        {
            return Results.NotFound(new EmailConfirmationResponseDto
            {
                Success = false,
                Message = "The account could not be found."
            });
        }

        if (user.EmailConfirmed)
        {
            return Results.Ok(new EmailConfirmationResponseDto
            {
                Success = true,
                Message = "The email address has already been confirmed.",
                Email = user.Email
            });
        }

        var confirmationResult = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!confirmationResult.Succeeded)
        {
            return Results.BadRequest(new EmailConfirmationResponseDto
            {
                Success = false,
                Message = "The confirmation link is invalid or has expired."
            });
        }

        return Results.Ok(new EmailConfirmationResponseDto
        {
            Success = true,
            Message = "Email address confirmed successfully.",
            Email = user.Email
        });
    }
}