using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using UserManagement.AM.ResponseDTO;

namespace UserManagement.Web.Handlers;

public static class ProfileHandler
{
    public static async Task<IResult> HandleAsync(
        HttpContext httpContext,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.GetUserAsync(httpContext.User);
        if (user is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new ProfileResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName!,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber
        });
    }
}