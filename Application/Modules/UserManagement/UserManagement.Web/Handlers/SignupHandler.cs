using FileProcessing.Infrastructure.Email.Abstraction.EmailTemplateAbstraction;
using FileProcessing.Infrastructure.Email.Models;
using FileProcessing.Infrastructure.Email.Settings;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using FileProcessing.Infrastructure.TaskQueue.SignupEmailVerifyTask;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using UserManagement.AM.RequestDTO;
using UserManagement.AM.ResponseDTO;

namespace UserManagement.Web.Handlers;

public static class SignupHandler
{
    public static async Task<IResult> HandleAsync(
        SignupRequestDto request,
        IValidator<SignupRequestDto> validator,
        UserManager<ApplicationUser> userManager,
        IEmailVerifyQueue emailVerifyQueue,
        IAppLogoProvider appLogoProvider,
        IOptions<EmailUrls> emailUrlsOptions,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

            return Results.ValidationProblem(validationErrors);
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = false
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var identityErrors = createResult.Errors
                .GroupBy(error => error.Code)
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());

            if (createResult.Errors.Any(error => error.Code is "DuplicateEmail" or "DuplicateUserName"))
            {
                return Results.Conflict(new SignupResponseDto
                {
                    Success = false,
                    Message = "An account with this email or username already exists."
                });
            }

            return Results.ValidationProblem(identityErrors);
        }

        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var emailUrls = emailUrlsOptions.Value;
        var confirmationUrl = QueryHelpers.AddQueryString(
            emailUrls.VerificationUrl,
            new Dictionary<string, string?>
            {
                ["userId"] = user.Id,
                ["token"] = confirmationToken
            });

        var lightLogo = await appLogoProvider.GetAppLightLogoAsync();
        var darkLogo = await appLogoProvider.GetAppDarkLogoAsync();

        var emailModel = new UserSignUpModel
        {
            UserName = request.UserName,
            Subject = "Verify your email address",
            To = request.Email,
            Message = "Please verify your email address to activate your account.",
            ActionText = "Verify email address",
            ActionUrl = confirmationUrl,
            TextBody = $"Hello {request.UserName}, please verify your email address: {confirmationUrl}",
            ContactUsPageUrl = emailUrls.ContactUsPageUrl,
            LogoBaseLightString = $"data:image/png;base64,{Convert.ToBase64String(lightLogo)}",
            LogoBaseDarkString = $"data:image/png;base64,{Convert.ToBase64String(darkLogo)}",
            HelpUrl = emailUrls.HelpUrl,
            Report_Link = emailUrls.ReportLink,
            Twitter_Link = emailUrls.TwitterLink,
            LinkedIn_Link = emailUrls.LinkedInLink,
            GitHub_or_Website_Link = emailUrls.WebsiteLink
        };

        if (!await emailVerifyQueue.AddJob(emailModel))
        {
            return Results.Problem(
                "The account was created, but its verification email could not be queued.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        return Results.Ok(new SignupResponseDto
        {
            Success = true,
            Message = "Account created. Please verify your email address.",
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email
        });
    }
}