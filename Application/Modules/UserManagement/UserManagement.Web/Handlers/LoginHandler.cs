using FileProcessing.Infrastructure.Jwt.CookieSetting;
using FileProcessing.Infrastructure.Jwt.Dto;
using FileProcessing.Infrastructure.Jwt.Token_Gen;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using UserManagement.AM.RequestDTO;
using UserManagement.AM.ResponseDTO;

namespace UserManagement.Web.Handlers;

public static class LoginHandler
{
    public static async Task<IResult> HandleAsync(
        LoginRequestDto request,
        IValidator<LoginRequestDto> validator,
        UserManager<ApplicationUser> userManager,
        ITokens tokens,
        ICookieConfiguration cookieConfiguration,
        AppDbContext dbContext,
        HttpContext httpContext,
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

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || await userManager.IsLockedOutAsync(user))
        {
            return InvalidCredentials();
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            return InvalidCredentials();
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var refreshToken = await tokens.RefreshToken<RefreshTokenDto>();
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        var deviceType = GetDeviceType(userAgent);
        var deviceName = httpContext.Request.Headers["X-Device-Name"].ToString().Trim();
        var session = new UserSession
        {
            UserId = user.Id,
            SessionId = Guid.NewGuid().ToString("N"),
            DeviceName = string.IsNullOrWhiteSpace(deviceName) ? deviceType : deviceName,
            DeviceType = deviceType,
            IpAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = userAgent,
            RefreshTokenHash = RefreshTokenHash.Compute(refreshToken.RFToken),
            CreatedAt = refreshToken.RFCreatedToken,
            LastUsedAt = refreshToken.RFCreatedToken,
            ExpiresAt = refreshToken.RFExpireToken
        };
        dbContext.Set<UserSession>().Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        var accessToken = await tokens.AccessToken(user, session.SessionId);

        httpContext.Response.Cookies.Append(
            "accessToken",
            accessToken,
            cookieConfiguration.AccesTokenCookieOptions());

        httpContext.Response.Cookies.Append(
            "refreshToken",
            refreshToken.RFToken,
            cookieConfiguration.RefreshTokenCookieOptions());

        return Results.Ok(new LoginResponseDto
        {
            Success = true,
            Message = "Login successful.",
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email
        });
    }

    private static IResult InvalidCredentials()
    {
        return Results.Json(
            new LoginResponseDto
            {
                Success = false,
                Message = "Invalid email or password."
            },
            statusCode: StatusCodes.Status401Unauthorized);
    }

    private static string GetDeviceType(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return "Unknown";
        }

        if (userAgent.Contains("ipad", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("tablet", StringComparison.OrdinalIgnoreCase))
        {
            return "Tablet";
        }

        return userAgent.Contains("mobile", StringComparison.OrdinalIgnoreCase) ||
               userAgent.Contains("iphone", StringComparison.OrdinalIgnoreCase)
            ? "Mobile"
            : "Desktop";
    }

}