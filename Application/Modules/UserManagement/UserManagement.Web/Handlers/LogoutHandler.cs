using FileProcessing.Infrastructure.Jwt.CookieSetting;
using Microsoft.AspNetCore.Http;

namespace UserManagement.Web.Handlers;

public static class LogoutHandler
{
    public static IResult HandleAsync(HttpContext httpContext, ICookieConfiguration cookieConfiguration)
    {
        httpContext.Response.Cookies.Delete("accessToken", cookieConfiguration.AccesTokenCookieOptions());
        httpContext.Response.Cookies.Delete("refreshToken", cookieConfiguration.RefreshTokenCookieOptions());
        return Results.NoContent();
    }
}