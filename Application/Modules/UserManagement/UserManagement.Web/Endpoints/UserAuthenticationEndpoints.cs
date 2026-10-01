using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UserManagement.Web.Handlers;

namespace UserManagement.Web;

public static class UserAuthenticationEndpoints
{
    public static IEndpointRouteBuilder MapUserAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authentication = endpoints.MapGroup("/api/v1/auth");

        authentication.MapPost("/sign-up", SignupHandler.HandleAsync).RequireRateLimiting("IP_AND_USERAGENT_FIXED");
        authentication.MapPost("/signup", SignupHandler.HandleAsync).RequireRateLimiting("IP_AND_USERAGENT_FIXED");
        authentication.MapGet("/verify-email", EmailConfirmationHandler.HandleAsync).AllowAnonymous().RequireRateLimiting("IP_AND_USERAGENT_FIXED");
        authentication.MapGet("/profile", ProfileHandler.HandleAsync).RequireAuthorization("User").RequireRateLimiting("Multilayer_Fixed");
        authentication.MapPost("/sign-in", LoginHandler.HandleAsync).RequireRateLimiting("IP_AND_USERAGENT_FIXED");
        authentication.MapPost("/signin", LoginHandler.HandleAsync).RequireRateLimiting("IP_AND_USERAGENT_FIXED");

        authentication.MapPost("/login", LoginHandler.HandleAsync).RequireRateLimiting("IP_AND_USERAGENT_FIXED");
        authentication.MapPost("/log-in", LoginHandler.HandleAsync).RequireRateLimiting("IP_AND_USERAGENT_FIXED");
        authentication.MapPost("/logout", LogoutHandler.HandleAsync).RequireAuthorization().RequireRateLimiting("Multilayer_Fixed");
        authentication.MapPost("/refresh-token", RefreshTokenHandler.HandleAsync).AllowAnonymous().RequireRateLimiting("IP_AND_USERAGENT_FIXED");
        authentication.MapPost("/sessions/terminate-others", TerminateOtherSessionsHandler.HandleAsync).RequireAuthorization().RequireRateLimiting("Multilayer_Fixed");

        return endpoints;
    }
}