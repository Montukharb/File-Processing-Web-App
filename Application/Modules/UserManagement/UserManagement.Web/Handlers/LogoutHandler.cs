using System.Security.Claims;
using FileProcessing.Infrastructure.Jwt.CookieSetting;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Web.Handlers;

public static class LogoutHandler
{
    public static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        ICookieConfiguration cookieConfiguration,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessionId = httpContext.User.FindFirstValue(ClaimTypes.Sid);
        if (userId is not null && sessionId is not null)
        {
            var session = await dbContext.Set<UserSession>()
                .SingleOrDefaultAsync(
                    item => item.UserId == userId && item.SessionId == sessionId,
                    cancellationToken);
            if (session is not null && !session.IsRevoked)
            {
                var now = DateTime.UtcNow;
                session.IsRevoked = true;
                session.RevokedAt = now;
                session.LastUsedAt = now;
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        httpContext.Response.Cookies.Delete("accessToken", cookieConfiguration.AccesTokenCookieOptions());
        httpContext.Response.Cookies.Delete("refreshToken", cookieConfiguration.RefreshTokenCookieOptions());
        return Results.NoContent();
    }
}