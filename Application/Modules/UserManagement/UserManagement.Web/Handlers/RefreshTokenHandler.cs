using FileProcessing.Infrastructure.Jwt.CookieSetting;
using FileProcessing.Infrastructure.Jwt.Dto;
using FileProcessing.Infrastructure.Jwt.Token_Gen;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Web.Handlers;

public static class RefreshTokenHandler
{
    public static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        AppDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        ITokens tokens,
        ICookieConfiguration cookieConfiguration,
        CancellationToken cancellationToken)
    {
        if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out var presentedToken) ||
            string.IsNullOrWhiteSpace(presentedToken))
        {
            return InvalidRefreshToken(httpContext, cookieConfiguration);
        }

        var tokenHash = RefreshTokenHash.Compute(presentedToken);
        var session = await dbContext.Set<UserSession>()
            .SingleOrDefaultAsync(item => item.RefreshTokenHash == tokenHash, cancellationToken);
        var now = DateTime.UtcNow;
        if (session is null || session.IsRevoked || session.RevokedAt.HasValue || session.ExpiresAt <= now)
        {
            return InvalidRefreshToken(httpContext, cookieConfiguration);
        }

        var user = await userManager.FindByIdAsync(session.UserId);
        if (user is null)
        {
            session.IsRevoked = true;
            session.RevokedAt = now;
            await dbContext.SaveChangesAsync(cancellationToken);
            return InvalidRefreshToken(httpContext, cookieConfiguration);
        }

        var refreshToken = await tokens.RefreshToken<RefreshTokenDto>();
        var rotated = await dbContext.Set<UserSession>()
            .Where(item => item.Id == session.Id &&
                           item.RefreshTokenHash == tokenHash &&
                           !item.IsRevoked &&
                           item.RevokedAt == null &&
                           item.ExpiresAt > now)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(item => item.RefreshTokenHash, RefreshTokenHash.Compute(refreshToken.RFToken))
                    .SetProperty(item => item.LastUsedAt, now)
                    .SetProperty(item => item.ExpiresAt, refreshToken.RFExpireToken),
                cancellationToken);
        if (rotated != 1)
        {
            return InvalidRefreshToken(httpContext, cookieConfiguration);
        }

        var accessToken = await tokens.AccessToken(user, session.SessionId);
        httpContext.Response.Cookies.Append(
            "accessToken",
            accessToken,
            cookieConfiguration.AccesTokenCookieOptions());
        httpContext.Response.Cookies.Append(
            "refreshToken",
            refreshToken.RFToken,
            cookieConfiguration.RefreshTokenCookieOptions());

        return Results.NoContent();
    }

    private static IResult InvalidRefreshToken(HttpContext httpContext, ICookieConfiguration cookieConfiguration)
    {
        httpContext.Response.Cookies.Delete("accessToken", cookieConfiguration.AccesTokenCookieOptions());
        httpContext.Response.Cookies.Delete("refreshToken", cookieConfiguration.RefreshTokenCookieOptions());
        return Results.Unauthorized();
    }
}