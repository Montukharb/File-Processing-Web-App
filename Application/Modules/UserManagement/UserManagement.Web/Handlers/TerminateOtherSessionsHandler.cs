using System.Security.Claims;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace UserManagement.Web.Handlers;

public static class TerminateOtherSessionsHandler
{
    public static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentSessionId = httpContext.User.FindFirstValue(ClaimTypes.Sid);
        if (userId is null || currentSessionId is null)
        {
            return Results.Unauthorized();
        }

        var sessions = await dbContext.Set<UserSession>()
            .Where(session => session.UserId == userId &&
                              session.SessionId != currentSessionId &&
                              !session.IsRevoked &&
                              session.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var session in sessions)
        {
            session.IsRevoked = true;
            session.RevokedAt = now;
            session.LastUsedAt = now;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(new { terminatedSessions = sessions.Count });
    }
}